using System.Text.Json;
using System.Text.RegularExpressions;
using FluentAssertions;

namespace GameHaven.UnitTests.Seo;

/// <summary>
/// Discoverability baseline of the public demo (REQ-013 to REQ-018). The site is the static
/// Vue client published to GitHub Pages, so the checks read the files the browser receives.
/// </summary>
public sealed class SeoBaselineTests
{
    private const string HostRoot = "https://gamehaven.naindev.com/";
    private const string AuthorId = "https://www.naindev.com/#person";

    private static readonly string ClientDir = Path.Combine(FindRepositoryRoot(), "GameHaven.Client");
    private static readonly string PublicDir = Path.Combine(ClientDir, "public");
    private static readonly string Html = File.ReadAllText(Path.Combine(ClientDir, "index.html"));

    [Fact]
    public void Canonical_Is_The_Host_Root()
    {
        Matches(@"<link\s+rel=""canonical""\s+href=""([^""]*)""")
            .Should().Equal(HostRoot);
    }

    [Fact]
    public void Description_Is_Single_And_Within_Snippet_Length()
    {
        var descriptions = MetaContent("name", "description");

        descriptions.Should().ContainSingle();
        descriptions[0].Length.Should().BeInRange(70, 160);
    }

    [Fact]
    public void Open_Graph_Matches_The_Canonical()
    {
        MetaContent("property", "og:type").Should().Equal("website");
        MetaContent("property", "og:site_name").Should().Equal("GameHaven");
        MetaContent("property", "og:url").Should().Equal(HostRoot);
        MetaContent("property", "og:title").Should().ContainSingle();
        MetaContent("property", "og:description").Should().Equal(MetaContent("name", "description"));
        MetaContent("property", "og:image").Should().ContainSingle().Which.Should().StartWith("https://");
    }

    [Fact]
    public void Twitter_Card_Is_Complete()
    {
        MetaContent("name", "twitter:card").Should().Equal("summary_large_image");
        MetaContent("name", "twitter:title").Should().ContainSingle();
        MetaContent("name", "twitter:description").Should().ContainSingle();
        MetaContent("name", "twitter:image").Should().ContainSingle();
    }

    [Fact]
    public void Author_Link_Points_To_The_Main_Site()
    {
        Matches(@"<link\s+rel=""author""\s+href=""([^""]*)""")
            .Should().Equal("https://www.naindev.com/");
    }

    [Fact]
    public void JsonLd_Parses_And_Names_The_Author()
    {
        var blocks = Matches(@"<script\s+type=""application/ld\+json"">(.*?)</script>", RegexOptions.Singleline);

        blocks.Should().ContainSingle();
        blocks[0].Should().NotContain("<");

        using var json = JsonDocument.Parse(blocks[0]);
        var root = json.RootElement;
        root.GetProperty("@type").GetString().Should().Be("SoftwareApplication");
        root.GetProperty("url").GetString().Should().Be(HostRoot);
        root.GetProperty("isAccessibleForFree").GetBoolean().Should().BeTrue();
        root.GetProperty("author").GetProperty("@id").GetString().Should().Be(AuthorId);
        root.GetProperty("sameAs").GetString().Should().Be("https://github.com/Nain9Dev/GameHaven");
    }

    [Fact]
    public void Robots_Advertises_The_Sitemap_And_Blocks_Nothing()
    {
        var robots = File.ReadAllText(Path.Combine(PublicDir, "robots.txt"));

        robots.Should().MatchRegex(@"(?m)^User-agent: \*\r?$");
        robots.Should().MatchRegex(@"(?m)^Allow: /\r?$");
        robots.Should().NotMatchRegex(@"(?m)^Disallow: *\S");
        robots.Should().Contain($"Sitemap: {HostRoot}sitemap.xml");
    }

    [Fact]
    public void Sitemap_Lists_The_Canonical_Root()
    {
        var sitemap = File.ReadAllText(Path.Combine(PublicDir, "sitemap.xml"));

        Regex.Matches(sitemap, "<loc>([^<]+)</loc>").Select(m => m.Groups[1].Value)
            .Should().Equal(HostRoot);
    }

    [Fact]
    public void LlmsTxt_Starts_With_A_Heading()
    {
        var lines = File.ReadAllLines(Path.Combine(PublicDir, "llms.txt"));

        lines[0].Should().Be("# GameHaven");
        lines.Should().Contain(l => l.StartsWith("> "));
        lines.Should().Contain(l => l.Contains("https://www.naindev.com/"));
    }

    private static List<string> Matches(string pattern, RegexOptions options = RegexOptions.None) =>
        Regex.Matches(Html, pattern, options).Select(m => m.Groups[1].Value).ToList();

    private static List<string> MetaContent(string attribute, string value) =>
        Matches($@"<meta\s+{attribute}=""{Regex.Escape(value)}""\s+content=""([^""]*)""");

    private static string FindRepositoryRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "GameHaven.slnx")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new DirectoryNotFoundException("GameHaven.slnx not found above the test output.");
    }
}

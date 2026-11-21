using System.Buffers.Binary;
using System.Text.Json;
using System.Text.RegularExpressions;
using FluentAssertions;

namespace GameHaven.UnitTests.Seo;

/// <summary>
/// Install contract of the public demo (INS-001 to INS-005): manifest, icon files and head tags.
/// Opacity and the maskable safe zone are checked by the icon generator, not here.
/// </summary>
public sealed class PwaInstallTests
{
    private static readonly string ClientDir = Path.Combine(FindRepositoryRoot(), "GameHaven.Client");
    private static readonly string PublicDir = Path.Combine(ClientDir, "public");
    private static readonly string Html = File.ReadAllText(Path.Combine(ClientDir, "index.html"));

    [Fact]
    public void Manifest_Names_The_App_And_Opens_It_Standalone()
    {
        using var document = ReadManifest();
        var root = document.RootElement;

        root.GetProperty("id").GetString().Should().Be("./");
        root.GetProperty("start_url").GetString().Should().Be("./");
        root.GetProperty("scope").GetString().Should().Be("./");
        root.GetProperty("display").GetString().Should().Be("standalone");
        root.GetProperty("name").GetString().Should().Be("GameHaven");
        root.GetProperty("short_name").GetString().Should().Be("GameHaven");
        root.GetProperty("description").GetString().Should().Be(MetaContent("name", "description").Single());
        root.GetProperty("lang").GetString().Should().Be("en");
        root.GetProperty("dir").GetString().Should().Be("ltr");
        root.GetProperty("theme_color").GetString().Should().Be("#090a0f");
        root.GetProperty("background_color").GetString().Should().Be("#090a0f");
        root.TryGetProperty("prefer_related_applications", out _).Should().BeFalse();
    }

    [Fact]
    public void Manifest_Icons_Exist_At_The_Declared_Size()
    {
        using var document = ReadManifest();
        var icons = document.RootElement.GetProperty("icons").EnumerateArray()
            .Select(icon => (
                Src: icon.GetProperty("src").GetString()!,
                Sizes: icon.GetProperty("sizes").GetString()!,
                Type: icon.GetProperty("type").GetString()!,
                Purpose: icon.GetProperty("purpose").GetString()!))
            .ToList();

        icons.Should().Equal(
            ("./icons/icon-192.png", "192x192", "image/png", "any"),
            ("./icons/icon-512.png", "512x512", "image/png", "any"),
            ("./icons/icon-maskable-512.png", "512x512", "image/png", "maskable"));

        foreach (var icon in icons)
        {
            PngSize(Path.Combine(PublicDir, icon.Src)).Should().Be(icon.Sizes, icon.Src);
        }

        PngSize(Path.Combine(PublicDir, "icons", "apple-touch-icon.png")).Should().Be("180x180");
    }

    [Fact]
    public void Page_Links_The_Manifest_And_Carries_The_Ios_Tags()
    {
        Matches(@"<link\s+rel=""manifest""\s+href=""([^""]*)""").Should().Equal("/manifest.webmanifest");
        Matches(@"<link\s+rel=""apple-touch-icon""\s+href=""([^""]*)""").Should().Equal("/icons/apple-touch-icon.png");
        MetaContent("name", "mobile-web-app-capable").Should().Equal("yes");
        MetaContent("name", "apple-mobile-web-app-capable").Should().Equal("yes");
        MetaContent("name", "apple-mobile-web-app-title").Should().Equal("GameHaven");
        MetaContent("name", "apple-mobile-web-app-status-bar-style").Should().Equal("black");
        MetaContent("name", "theme-color").Should().Equal("#090a0f");
    }

    private static JsonDocument ReadManifest() =>
        JsonDocument.Parse(File.ReadAllText(Path.Combine(PublicDir, "manifest.webmanifest")));

    // Reads width and height from the IHDR chunk, so no image library is needed.
    private static string PngSize(string path)
    {
        var header = File.ReadAllBytes(path).AsSpan(0, 24);
        header[..8].ToArray().Should().Equal(0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A);
        var width = BinaryPrimitives.ReadUInt32BigEndian(header[16..20]);
        var height = BinaryPrimitives.ReadUInt32BigEndian(header[20..24]);
        return $"{width}x{height}";
    }

    private static List<string> MetaContent(string attribute, string name) =>
        Matches($@"<meta\s+{attribute}=""{Regex.Escape(name)}""\s+content=""([^""]*)""");

    private static List<string> Matches(string pattern) =>
        Regex.Matches(Html, pattern).Select(match => match.Groups[1].Value).ToList();

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "GameHaven.slnx")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName ?? throw new DirectoryNotFoundException("GameHaven.slnx not found above the test output.");
    }
}

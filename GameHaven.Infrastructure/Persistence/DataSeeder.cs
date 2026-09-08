using System.Data.Common;
using GameHaven.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameHaven.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(GameHavenDbContext context)
    {
        await EnsureUpToDateSchemaAsync(context);

        if (await context.Games.AnyAsync())
            return;

        var games = CatalogSeedData.Entries.Select(ToGame).ToList();

        await context.Games.AddRangeAsync(games);
        await context.SaveChangesAsync();
    }

    private static Game ToGame(CatalogSeedEntry entry)
    {
        var game = new Game(
            entry.Title,
            entry.Tagline,
            entry.Price,
            entry.Studio,
            entry.Studio,
            DateTime.UtcNow.AddDays(-entry.DaysSinceRelease),
            entry.Cover,
            entry.Genre,
            entry.Rating);

        if (entry.Discount > 0)
            game.ApplyDiscount(entry.Discount);

        return game;
    }

    /// <summary>
    /// The demo runs on a throwaway SQLite file created by EnsureCreated, which never
    /// migrates an existing database. If a file written by an older build survives a
    /// redeploy its schema no longer matches the model, so probe the columns once and
    /// rebuild the file when the probe fails.
    /// </summary>
    private static async Task EnsureUpToDateSchemaAsync(GameHavenDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        try
        {
            _ = await context.Games
                .Select(g => new { g.Id, g.Genre, g.RatingPercentage })
                .FirstOrDefaultAsync();
        }
        catch (DbException)
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }
    }
}

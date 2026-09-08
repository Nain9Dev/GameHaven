using GameHaven.Application.Interfaces;
using GameHaven.Domain.Entities;
using GameHaven.Infrastructure;
using GameHaven.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddInfrastructure("Data Source=gamehaven.db");

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(policyName: "fixed", config =>
    {
        config.PermitLimit = 60;
        config.Window = TimeSpan.FromMinutes(1);
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        config.QueueLimit = 2;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GameHavenDbContext>();
    await DataSeeder.SeedAsync(context);
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseRateLimiter();

// ---- ENDPOINTS ----

// Cheap unauthenticated probe. The public demo pings it on load to wake the
// free-tier container while the storefront renders its offline snapshot.
app.MapGet("/api/health", () => Results.Ok(new { status = "ok", utc = DateTime.UtcNow }))
    .WithName("HealthCheck");

app.MapGet("/api/games", async (IGameRepository gameRepo) =>
{
    var games = await gameRepo.GetAllAsync();
    return Results.Ok(games.Select(ToCatalogEntry));
})
.WithName("GetCatalog")
.RequireRateLimiting("fixed");

app.MapGet("/api/games/{id:guid}", async (Guid id, IGameRepository gameRepo) =>
{
    var game = await gameRepo.GetByIdAsync(id);
    return game is null ? Results.NotFound() : Results.Ok(ToCatalogEntry(game));
})
.WithName("GetGame")
.RequireRateLimiting("fixed");

app.MapGet("/api/library/{userId:guid}", async (Guid userId, ILibraryRepository libRepo) =>
{
    var library = await libRepo.GetByUserIdAsync(userId);
    if (library == null) return Results.Ok(new { UserId = userId, OwnedGames = Array.Empty<object>() });

    return Results.Ok(new
    {
        library.UserId,
        OwnedGames = library.OwnedGames.Select(g => new { g.GameId, g.PurchaseDate, g.PlayTimeMinutes })
    });
})
.WithName("GetUserLibrary")
.RequireRateLimiting("fixed");

app.MapGet("/api/cart/{userId:guid}", async (Guid userId, ICartRepository cartRepo) =>
{
    var cart = await cartRepo.GetByUserIdAsync(userId);
    if (cart == null) return Results.Ok(new { UserId = userId, Items = Array.Empty<object>(), TotalAmount = 0 });

    return Results.Ok(new
    {
        cart.UserId,
        cart.TotalAmount,
        Items = cart.Items.Select(i => new { i.GameId, i.PriceAtAddedTime })
    });
})
.WithName("GetCart")
.RequireRateLimiting("fixed");

app.MapPost("/api/cart/{userId:guid}/add", async (
    Guid userId,
    [FromBody] AddToCartRequest req,
    ICartRepository cartRepo,
    ILibraryRepository libRepo,
    IGameRepository gameRepo,
    GameHavenDbContext db) =>
{
    var game = await gameRepo.GetByIdAsync(req.GameId);
    if (game == null) return Results.NotFound("Game not found.");

    var library = await libRepo.GetByUserIdAsync(userId);
    if (library != null && library.HasGame(req.GameId))
        return Results.BadRequest("You already own this game.");

    var cart = await cartRepo.GetByUserIdAsync(userId);
    if (cart == null)
    {
        cart = new Cart(userId);
        await cartRepo.AddAsync(cart);
    }

    try
    {
        cart.AddItem(game.Id, game.CurrentPrice);
        await db.SaveChangesAsync();
        return Results.Ok();
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(ex.Message);
    }
})
.WithName("AddToCart")
.RequireRateLimiting("fixed");

app.MapPost("/api/cart/{userId:guid}/remove", async (
    Guid userId,
    [FromBody] AddToCartRequest req,
    ICartRepository cartRepo,
    GameHavenDbContext db) =>
{
    var cart = await cartRepo.GetByUserIdAsync(userId);
    if (cart == null) return Results.NotFound("Cart not found.");

    cart.RemoveItem(req.GameId);
    await db.SaveChangesAsync();
    return Results.Ok();
})
.WithName("RemoveFromCart")
.RequireRateLimiting("fixed");

app.MapPost("/api/cart/{userId:guid}/checkout", async (
    Guid userId,
    ICartRepository cartRepo,
    ILibraryRepository libRepo,
    GameHavenDbContext db) =>
{
    var cart = await cartRepo.GetByUserIdAsync(userId);
    if (cart == null || !cart.Items.Any()) return Results.BadRequest("Cart is empty.");

    var library = await libRepo.GetByUserIdAsync(userId);
    if (library == null)
    {
        library = new UserLibrary(userId);
        await libRepo.AddAsync(library);
    }

    foreach (var item in cart.Items)
    {
        // A cart can outlive a purchase of the same title in another tab, so skip
        // duplicates instead of letting the aggregate throw mid-checkout.
        if (!library.HasGame(item.GameId))
            library.AddGame(item.GameId);
    }

    cart.Clear();
    await db.SaveChangesAsync();

    return Results.Ok("Checkout successful.");
})
.WithName("Checkout")
.RequireRateLimiting("fixed");

app.Run();

static object ToCatalogEntry(Game game) => new
{
    game.Id,
    game.Title,
    game.Description,
    game.BasePrice,
    game.DiscountPercentage,
    game.CurrentPrice,
    game.CoverImageUrl,
    game.Developer,
    game.Publisher,
    game.Genre,
    game.RatingPercentage,
    game.ReleaseDate
};

public class AddToCartRequest
{
    public Guid GameId { get; set; }
}

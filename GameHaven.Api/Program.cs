using GameHaven.Infrastructure;
using GameHaven.Infrastructure.Persistence;
using GameHaven.Application.Interfaces;
using GameHaven.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddInfrastructure("Data Source=gamehaven.db");

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

// ---- ENDPOINTS ----

app.MapGet("/api/games", async (IGameRepository gameRepo) =>
{
    var games = await gameRepo.GetAllAsync();
    return Results.Ok(games.Select(g => new 
    {
        g.Id, g.Title, g.Description, g.BasePrice, g.DiscountPercentage, g.CurrentPrice, g.CoverImageUrl, g.Developer
    }));
})
.WithName("GetCatalog");

app.MapGet("/api/library/{userId}", async (Guid userId, ILibraryRepository libRepo) =>
{
    var library = await libRepo.GetByUserIdAsync(userId);
    if (library == null) return Results.Ok(new { UserId = userId, OwnedGames = Array.Empty<object>() });

    return Results.Ok(new 
    {
        library.UserId,
        OwnedGames = library.OwnedGames.Select(g => new { g.GameId, g.PurchaseDate, g.PlayTimeMinutes })
    });
})
.WithName("GetUserLibrary");

app.MapGet("/api/cart/{userId}", async (Guid userId, ICartRepository cartRepo) =>
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
.WithName("GetCart");

app.MapPost("/api/cart/{userId}/add", async (Guid userId, [FromBody] AddToCartRequest req, ICartRepository cartRepo, ILibraryRepository libRepo, IGameRepository gameRepo, GameHavenDbContext db) =>
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
    catch(Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
})
.WithName("AddToCart");

app.MapPost("/api/cart/{userId}/checkout", async (Guid userId, ICartRepository cartRepo, ILibraryRepository libRepo, GameHavenDbContext db) =>
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
        library.AddGame(item.GameId);
    }

    cart.Clear();
    await db.SaveChangesAsync();

    return Results.Ok("Checkout successful.");
})
.WithName("Checkout");

app.Run();

public class AddToCartRequest
{
    public Guid GameId { get; set; }
}

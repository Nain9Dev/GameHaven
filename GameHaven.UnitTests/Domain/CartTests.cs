using FluentAssertions;
using GameHaven.Domain.Entities;
using Xunit;

namespace GameHaven.UnitTests.Domain;

public class CartTests
{
    [Fact]
    public void AddItem_WhenCartIsEmpty_AddsItemAndUpdatesTotal()
    {
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        var cart = new Cart(userId);

        cart.AddItem(gameId, 19.99m);

        cart.Items.Should().HaveCount(1);
        cart.TotalAmount.Should().Be(19.99m);
        cart.Items.First().GameId.Should().Be(gameId);
        cart.Items.First().PriceAtAddedTime.Should().Be(19.99m);
    }

    [Fact]
    public void AddItem_WhenGameAlreadyInCart_ThrowsInvalidOperationException()
    {
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        var cart = new Cart(userId);

        cart.AddItem(gameId, 25.00m);

        var act = () => cart.AddItem(gameId, 25.00m);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Game is already in the cart*");
    }

    [Fact]
    public void RemoveItem_WhenGameExists_RemovesFromCartAndUpdatesTotal()
    {
        var userId = Guid.NewGuid();
        var gameA = Guid.NewGuid();
        var gameB = Guid.NewGuid();
        var cart = new Cart(userId);

        cart.AddItem(gameA, 10.00m);
        cart.AddItem(gameB, 20.00m);
        cart.TotalAmount.Should().Be(30.00m);

        cart.RemoveItem(gameA);

        cart.Items.Should().HaveCount(1);
        cart.TotalAmount.Should().Be(20.00m);
        cart.Items.First().GameId.Should().Be(gameB);
    }

    [Fact]
    public void Clear_RemovesAllItemsAndResetsTotal()
    {
        var userId = Guid.NewGuid();
        var cart = new Cart(userId);

        cart.AddItem(Guid.NewGuid(), 15.00m);
        cart.AddItem(Guid.NewGuid(), 35.00m);
        cart.TotalAmount.Should().Be(50.00m);

        cart.Clear();

        cart.Items.Should().BeEmpty();
        cart.TotalAmount.Should().Be(0m);
    }
}

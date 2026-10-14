using FluentAssertions;
using GameHaven.Domain.Entities;
using Xunit;

namespace GameHaven.UnitTests.Domain;

public class GameTests
{
    [Fact]
    public void Constructor_WithValidData_InstantiatesCorrectly()
    {
        var releaseDate = new DateTime(2025, 6, 15);
        var game = new Game(
            title: "Neon Odyssey",
            description: "A cyberpunk pixel-art adventure",
            basePrice: 29.99m,
            developer: "PixelForge Studios",
            publisher: "IndieArcade",
            releaseDate: releaseDate,
            coverImageUrl: "https://example.com/cover.png",
            genre: "Adventure",
            ratingPercentage: 92
        );

        game.Title.Should().Be("Neon Odyssey");
        game.BasePrice.Should().Be(29.99m);
        game.CurrentPrice.Should().Be(29.99m);
        game.DiscountPercentage.Should().Be(0);
        game.Genre.Should().Be("Adventure");
        game.RatingPercentage.Should().Be(92);
        game.ReleaseDate.Should().Be(releaseDate);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithInvalidTitle_ThrowsArgumentException(string? invalidTitle)
    {
        var act = () => new Game(
            title: invalidTitle!,
            description: "Valid Description",
            basePrice: 19.99m,
            developer: "Dev",
            publisher: "Pub",
            releaseDate: DateTime.UtcNow,
            coverImageUrl: "https://example.com/cover.png"
        );

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Title is required*");
    }

    [Fact]
    public void Constructor_WithNegativePrice_ThrowsArgumentException()
    {
        var act = () => new Game(
            title: "Retro Run",
            description: "Valid Description",
            basePrice: -5.00m,
            developer: "Dev",
            publisher: "Pub",
            releaseDate: DateTime.UtcNow,
            coverImageUrl: "https://example.com/cover.png"
        );

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Base price cannot be negative*");
    }

    [Theory]
    [InlineData(100.0, 20, 80.0)]
    [InlineData(50.0, 50, 25.0)]
    [InlineData(30.0, 0, 30.0)]
    [InlineData(10.0, 100, 0.0)]
    public void ApplyDiscount_CalculatesCurrentPriceAccurately(double basePrice, int discount, double expectedPrice)
    {
        var game = new Game(
            title: "Space Drifter",
            description: "Asteroid mining sim",
            basePrice: (decimal)basePrice,
            developer: "CosmoDev",
            publisher: "Nova",
            releaseDate: DateTime.UtcNow,
            coverImageUrl: "https://example.com/space.png"
        );

        game.ApplyDiscount(discount);

        game.DiscountPercentage.Should().Be(discount);
        game.CurrentPrice.Should().Be((decimal)expectedPrice);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void ApplyDiscount_WithOutOfBoundsPercentage_ThrowsArgumentException(int invalidDiscount)
    {
        var game = new Game(
            title: "Void",
            description: "Space mystery",
            basePrice: 15.00m,
            developer: "Dev",
            publisher: "Pub",
            releaseDate: DateTime.UtcNow,
            coverImageUrl: "https://example.com/void.png"
        );

        var act = () => game.ApplyDiscount(invalidDiscount);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Discount must be between 0 and 100*");
    }

    [Theory]
    [InlineData(-10, 0)]
    [InlineData(115, 100)]
    [InlineData(88, 88)]
    public void Constructor_ClampsRatingBetweenZeroAndHundred(int inputRating, int expectedRating)
    {
        var game = new Game(
            title: "Clamped Quest",
            description: "A quest for bounds",
            basePrice: 9.99m,
            developer: "Dev",
            publisher: "Pub",
            releaseDate: DateTime.UtcNow,
            coverImageUrl: "https://example.com/quest.png",
            ratingPercentage: inputRating
        );

        game.RatingPercentage.Should().Be(expectedRating);
    }
}

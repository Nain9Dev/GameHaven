using FluentAssertions;
using GameHaven.Domain.Entities;
using Xunit;

namespace GameHaven.UnitTests.Domain;

public class UserLibraryTests
{
    [Fact]
    public void AddGame_WhenNotOwned_AddsToLibrary()
    {
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        var library = new UserLibrary(userId);

        library.AddGame(gameId);

        library.OwnedGames.Should().HaveCount(1);
        library.HasGame(gameId).Should().BeTrue();
        library.OwnedGames.First().GameId.Should().Be(gameId);
    }

    [Fact]
    public void AddGame_WhenAlreadyOwned_ThrowsInvalidOperationException()
    {
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        var library = new UserLibrary(userId);

        library.AddGame(gameId);

        var act = () => library.AddGame(gameId);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*User already owns this game*");
    }

    [Fact]
    public void HasGame_WhenGameNotPresent_ReturnsFalse()
    {
        var userId = Guid.NewGuid();
        var library = new UserLibrary(userId);

        library.HasGame(Guid.NewGuid()).Should().BeFalse();
    }
}

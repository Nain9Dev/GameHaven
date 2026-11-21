using NetArchTest.Rules;
using Xunit;

namespace GameHaven.UnitTests.Architecture;

public sealed class CleanArchitectureTests
{
    private static readonly System.Reflection.Assembly DomainAssembly = typeof(GameHaven.Domain.Entities.Game).Assembly;
    private static readonly System.Reflection.Assembly ApplicationAssembly = typeof(GameHaven.Application.Interfaces.IGameRepository).Assembly;
    private static readonly System.Reflection.Assembly InfrastructureAssembly = typeof(GameHaven.Infrastructure.DependencyInjection).Assembly;
    private static readonly System.Reflection.Assembly ApiAssembly = typeof(AddToCartRequest).Assembly;

    [Fact]
    public void Domain_Should_Not_Have_Dependency_On_Other_Projects()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "GameHaven.Application",
                "GameHaven.Infrastructure",
                "GameHaven.Api",
                "GameHaven.DesktopUI",
                "GameHaven.Client")
            .GetResult();

        Assert.True(result.IsSuccessful, $"Domain layer has illegal dependencies on outer layers: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Application_Should_Not_Have_Dependency_On_Infrastructure_Or_Api_Or_UI()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "GameHaven.Infrastructure",
                "GameHaven.Api",
                "GameHaven.DesktopUI",
                "GameHaven.Client")
            .GetResult();

        Assert.True(result.IsSuccessful, $"Application layer has illegal dependencies: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Infrastructure_Should_Not_Have_Dependency_On_Api_Or_UI()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "GameHaven.Api",
                "GameHaven.DesktopUI",
                "GameHaven.Client")
            .GetResult();

        Assert.True(result.IsSuccessful, $"Infrastructure layer has illegal dependencies: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Domain_Entities_Should_Not_Depend_On_EntityFrameworkCore()
    {
        var result = Types.InAssembly(DomainAssembly)
            .That()
            .ResideInNamespace("GameHaven.Domain")
            .ShouldNot()
            .HaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        Assert.True(result.IsSuccessful, $"Domain entities must not depend on EF Core: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }
}

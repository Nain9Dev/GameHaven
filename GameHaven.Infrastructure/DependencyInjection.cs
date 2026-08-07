using GameHaven.Application.Interfaces;
using GameHaven.Infrastructure.Persistence;
using GameHaven.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GameHaven.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<GameHavenDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ILibraryRepository, LibraryRepository>();
        
        return services;
    }
}

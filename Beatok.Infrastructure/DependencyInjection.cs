using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Repositories;
using Beatok.Infrastructure.Authentication;
using Beatok.Infrastructure.BackgroundServices;
using Beatok.Infrastructure.Persistence;
using Beatok.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beatok.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("NeonConnection")));

        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddHostedService<InactiveUserCleanupService>();
        
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }
}
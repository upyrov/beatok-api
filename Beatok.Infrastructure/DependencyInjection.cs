using Amazon.S3;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Repositories;
using Beatok.Infrastructure.Authentication;
using Beatok.Infrastructure.BackgroundServices;
using Beatok.Infrastructure.Persistence;
using Beatok.Infrastructure.Persistence.Repositories;
using Hangfire;
using Hangfire.PostgreSql;
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
        services.AddHostedService<TokenCleanupService>();
        
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
      
        services.AddScoped<IKitRepository, KitRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ISoundRepository, SoundRepository>();
      
        services.AddScoped<ILobbyRepository, LobbyRepository>();
        services.AddScoped<IParticipationRepository, ParticipationRepository>();

        services.Configure<R2Options>(
            configuration.GetSection(nameof(R2Options)));
        
        services.AddSingleton<IAmazonS3>(_ =>
        {
            var r2Options = configuration.GetSection(nameof(R2Options)).Get<R2Options>();

            var config = new AmazonS3Config
            {
                ServiceURL = r2Options!.ServiceUrl,
                ForcePathStyle = true
            };

            return new AmazonS3Client(
                r2Options.AccessKey,
                r2Options.SecretKey,
                config);
        });
        
        services.AddSingleton<ISoundStorage, R2StorageSound>();
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddHangfire(h =>
            h.UsePostgreSqlStorage(options => 
                options.UseNpgsqlConnection(configuration.GetConnectionString("NeonConnection"))));
        services.AddHangfireServer();
        
        return services;
    }
}
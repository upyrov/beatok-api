using Amazon.S3;
using Beatok.Application.Interfaces;
using Beatok.Infrastructure.BackgroundServices;
using Beatok.Infrastructure.Persistence;
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
        
        services.AddScoped<IApplicationDbContext>(provider => 
            provider.GetRequiredService<ApplicationDbContext>());
        
        services.AddHostedService<InactiveUserCleanupService>();
        services.AddHostedService<LobbyCleanupService>();
        
        services.Configure<R2Options>(configuration.GetSection(nameof(R2Options)));
        
        services.AddSingleton<IAmazonS3>(_ =>
        {
            var r2Options = configuration.GetSection(nameof(R2Options)).Get<R2Options>();

            var config = new AmazonS3Config
            {
                ServiceURL = r2Options!.ServiceUrl,
                ForcePathStyle = true
            };

            return new AmazonS3Client(r2Options.AccessKey, r2Options.SecretKey, config);
        });
        
        services.AddSingleton<IStorage, R2Storage>();
        
        services.AddHangfire(h =>
            h.UsePostgreSqlStorage(options => 
                options.UseNpgsqlConnection(configuration.GetConnectionString("NeonConnection"))));
        services.AddHangfireServer();
        
        return services;
    }
}
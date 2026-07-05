using Beatok.Application.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Beatok.Infrastructure.BackgroundServices;

public class TokenCleanupService(IServiceProvider serviceProvider,
    ILogger<TokenCleanupService> logger): BackgroundService
{
    private readonly TimeSpan _period = TimeSpan.FromDays(1);
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(_period);
        
        while (await timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var refreshTokenRepository = scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();

                    int deletedCount = await refreshTokenRepository.DeleteExpiredAsync();
                    logger.LogInformation("Deleted {DeletedCount} expired tokens", deletedCount);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error cleaning up expired tokens");
            }
        }
    }
}
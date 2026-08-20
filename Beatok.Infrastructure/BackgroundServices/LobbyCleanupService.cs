using Beatok.Application.Interfaces;
using Beatok.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Beatok.Infrastructure.BackgroundServices;

public class LobbyCleanupService(
    IServiceProvider serviceProvider,
    ILogger<LobbyCleanupService> logger): BackgroundService
{
    private readonly TimeSpan _period  = TimeSpan.FromHours(1);
    
    private readonly TimeSpan _expirationThreshold = TimeSpan.FromHours(6);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(_period);

        while (await timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

                    var threshold = DateTime.UtcNow - _expirationThreshold;

                    int deletedCount = await context.Lobbies
                        .Where(l => l.State != LobbyState.Ended
                                    && l.CreatedAt < threshold)
                        .ExecuteDeleteAsync(stoppingToken);
                    logger.LogInformation("Deleted {DeletedCount} lobbies", deletedCount);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error cleaning up lobbies");
            }
        }
    }
}
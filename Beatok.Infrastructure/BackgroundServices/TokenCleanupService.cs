using Beatok.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
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
                    var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

                    int deletedCount = await context.RefreshTokens
                        .Where(t => t.Expires < DateTime.UtcNow)
                        .ExecuteDeleteAsync();
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
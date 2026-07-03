using Beatok.Application.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Beatok.Infrastructure.BackgroundServices;

public class InactiveUserCleanupService(
    IServiceProvider serviceProvider,
    ILogger<InactiveUserCleanupService> logger): BackgroundService
{
    private readonly TimeSpan _period = TimeSpan.FromDays(1);
    
    private readonly TimeSpan _expirationThreshold = TimeSpan.FromDays(30);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(_period);

        while (await timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                    var threshold = DateTime.UtcNow - _expirationThreshold;

                    int deletedCount = await userRepository.DeleteExpiredAnonymousUsersAsync(threshold);
                    logger.LogInformation("Deleted {DeletedCount} anonymous users", deletedCount);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error cleaning up expired anonymous users");
            }
        }
    }
}
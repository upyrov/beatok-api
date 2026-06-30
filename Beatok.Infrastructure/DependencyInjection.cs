using Microsoft.Extensions.DependencyInjection;

namespace Beatok.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Infrastructure services
        // DbContext
        return services;
    }
}
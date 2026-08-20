using Microsoft.Extensions.DependencyInjection;

namespace Summaries.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        return services;
    }
}
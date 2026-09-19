using System.Reflection;

using Summaries.API.Common.RateLimiting;
using Summaries.Shared.Infrastructure.Api;
using Summaries.API.Cors;
using Summaries.API.Versioning;

namespace Summaries.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration,
        params Assembly[] moduleControllerAssemblies)
    {
        services.AddApiControllers(moduleControllerAssemblies);
        services.AddApiVersioningSetup();
        services.AddOpenApi();
        services.AddApiCors(configuration);
        services.AddApiRateLimiting();

        services.AddSingleton(TimeProvider.System);

        return services;
    }
}

using Summaries.API.Controllers;
using Summaries.API.Cors;
using Summaries.API.Versioning;

namespace Summaries.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddApiControllers();
        services.AddApiVersioningSetup();
        services.AddOpenApi();
        services.AddApiCors(configuration);
        return services;
    }
}
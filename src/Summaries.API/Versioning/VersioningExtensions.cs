using Asp.Versioning;

namespace Summaries.API.Versioning;

public static class VersioningExtensions
{
    public static IServiceCollection AddApiVersioningSetup(
        this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1.0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        });

        return services;
    }
}
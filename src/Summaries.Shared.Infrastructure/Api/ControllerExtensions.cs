using System.Reflection;
using System.Text.Json.Serialization;

namespace Summaries.Shared.Infrastructure.Api;

public static class ControllerExtensions
{
    /// <summary>
    /// Registers MVC controllers found in the host assembly plus every
    /// module assembly passed in — each module's Api/Controllers live in
    /// that module's own project, so ASP.NET won't discover them unless
    /// their assembly is explicitly added as an ApplicationPart.
    /// </summary>
    public static IServiceCollection AddApiControllers(
        this IServiceCollection services,
        params Assembly[] moduleAssemblies)
    {
        var builder = services.AddControllers()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter()));

        foreach (var assembly in moduleAssemblies)
        {
            builder.AddApplicationPart(assembly);
        }

        return services;
    }
}
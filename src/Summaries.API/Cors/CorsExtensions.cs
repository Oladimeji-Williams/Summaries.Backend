namespace Summaries.API.Cors;

public static class CorsExtensions
{
    private const string PolicyName = "SummariesFrontend";

    public static IServiceCollection AddApiCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var allowedOrigins = configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>()
            ?? throw new InvalidOperationException(
                "Cors:AllowedOrigins is not configured.");

        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy =>
            {
                policy
                    .WithOrigins(allowedOrigins)
                    .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                    .WithHeaders("Content-Type", "Authorization")
                    .AllowCredentials();
            });
        });

        return services;
    }

    public static IApplicationBuilder UseApiCors(
        this IApplicationBuilder app)
    {
        return app.UseCors(PolicyName);
    }
}
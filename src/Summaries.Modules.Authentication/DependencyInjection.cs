using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.Modules.Authentication.Infrastructure.ExternalProviders;
using Summaries.Modules.Authentication.Infrastructure.Identity;
using Summaries.Modules.Authentication.Infrastructure.Tokens;
using Summaries.Modules.Authentication.Persistence;

namespace Summaries.Modules.Authentication;

/// <summary>
/// Authentication owns the user record (ASP.NET Identity's ApplicationUser)
/// because everything it does — login, 2FA, password resets — is
/// credential-shaped. Users/Admin/Books never reference this module's
/// project; they depend on IIdentityService (declared in SharedKernel),
/// which this module implements.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddAuthenticationModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<ExternalAuthOptions>(configuration.GetSection(ExternalAuthOptions.SectionName));

        services.AddDbContext<ApplicationIdentityDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
                sql.MigrationsHistoryTable("__EFMigrationsHistory_Identity")));

        services.AddIdentityConfiguration();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddAuthenticationConfiguration(configuration);

        return services;
    }
}

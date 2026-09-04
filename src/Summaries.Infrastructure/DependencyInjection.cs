using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Resend;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Abstractions.Storage;
using Summaries.Infrastructure.Authentication;
using Summaries.Infrastructure.Identity;
using Summaries.Infrastructure.Storage;
using Summaries.Infrastructure.Email;
using Summaries.Application.Abstractions.Email;


namespace Summaries.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found.");

        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));

        services.AddDbContext<ApplicationIdentityDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
                sql.MigrationsHistoryTable("__EFMigrationsHistory_Identity")));
        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
        services.AddHttpClient<IResend, ResendClient>();
        services.Configure<ResendClientOptions>(o =>
        {
            o.ApiToken = configuration["Email:ApiKey"]
                ?? throw new InvalidOperationException("Email:ApiKey is not configured.");
        });
        services.AddScoped<IEmailSender, ResendEmailSender>();

        services.AddHttpContextAccessor();
        services.AddIdentityConfiguration();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddAuthenticationConfiguration(configuration);
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<IImageValidator, ImageValidator>();
        return services;
    }
}
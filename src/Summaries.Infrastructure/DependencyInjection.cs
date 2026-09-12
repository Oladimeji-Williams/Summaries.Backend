using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Resend;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Abstractions.Email;
using Summaries.Application.Abstractions.Storage;
using Summaries.Infrastructure.Authentication;
using Summaries.Infrastructure.Email;
using Summaries.Infrastructure.Identity;
using Summaries.Infrastructure.Storage;
using Summaries.Infrastructure.Payments;
using Summaries.Application.Abstractions.Payments;


namespace Summaries.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
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

        services.AddHttpContextAccessor();
        services.AddIdentityConfiguration();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddAuthenticationConfiguration(configuration);

        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
        services.Configure<BrandingOptions>(configuration.GetSection(BrandingOptions.SectionName));
        services.AddHttpClient<IResend, ResendClient>();
        services.Configure<ResendClientOptions>(o =>
        {
            o.ApiToken = configuration["Email:ApiKey"]
                ?? throw new InvalidOperationException("Email:ApiKey is not configured.");
        });
        services.AddHttpClient<IEmailSender, ResendEmailSender>();

        services.Configure<CloudinaryOptions>(configuration.GetSection(CloudinaryOptions.SectionName));
        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<CloudinaryOptions>>().Value;
            var account = new Account(options.CloudName, options.ApiKey, options.ApiSecret);
            return new Cloudinary(account)
                {
                    Api = { Secure = true }
                };
        });
        services.AddScoped<IFileStorageService, CloudinaryFileStorageService>();
        services.AddScoped<IImageValidator, ImageValidator>();
        services.Configure<PaystackOptions>(configuration.GetSection(PaystackOptions.SectionName));
        services.AddHttpClient<IPaystackService, PaystackService>(client =>
        {
            client.BaseAddress = new Uri("https://api.paystack.co/");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", configuration["Paystack:SecretKey"]);
        });       

        return services;
    }
}
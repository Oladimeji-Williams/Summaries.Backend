using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Resend;

using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.SharedKernel.Abstractions.Email;
using Summaries.SharedKernel.Abstractions.Storage;
using Summaries.Shared.Infrastructure.Auditing;
using Summaries.Shared.Infrastructure.Authentication;
using Summaries.Shared.Infrastructure.Email;
using Summaries.Shared.Infrastructure.Storage;
using Summaries.Shared.Infrastructure.Urls;

namespace Summaries.Shared.Infrastructure;

/// <summary>
/// Technology every module might need but none of them owns: email, file
/// storage, image validation, "who is calling" (reads claims off
/// HttpContext), and building absolute URLs. Module-specific technology
/// (JWT issuing, ASP.NET Identity, Paystack) lives inside the module that
/// owns it instead — see Modules.Authentication / Modules.Payments.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddSharedInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IUrlBuilder, Summaries.Shared.Infrastructure.Urls.UrlBuilder>();
        services.AddSingleton<AuditingInterceptor>();

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
            var options = sp.GetRequiredService<IOptions<CloudinaryOptions>>().Value;
            var account = new Account(options.CloudName, options.ApiKey, options.ApiSecret);
            return new Cloudinary(account)
            {
                Api = { Secure = true }
            };
        });
        services.AddScoped<IFileStorageService, CloudinaryFileStorageService>();
        services.AddScoped<IImageValidator, ImageValidator>();

        return services;
    }
}

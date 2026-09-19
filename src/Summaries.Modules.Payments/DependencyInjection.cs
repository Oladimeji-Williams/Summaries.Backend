using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Summaries.SharedKernel.Abstractions.Payments;
using Summaries.SharedKernel.Contracts.Payments;
using Summaries.Shared.Infrastructure.Auditing;
using Summaries.Modules.Payments.Application.Abstractions;
using Summaries.Modules.Payments.Application.CrossModule;
using Summaries.Modules.Payments.Infrastructure.Paystack;
using Summaries.Modules.Payments.Persistence;
using Summaries.Modules.Payments.Persistence.Repositories;

namespace Summaries.Modules.Payments;

public static class DependencyInjection
{
    public static IServiceCollection AddPaymentsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        services.AddDbContext<PaymentsDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(connectionString, sql =>
                sql.MigrationsHistoryTable("__EFMigrationsHistory_Payments"));

            options.AddInterceptors(serviceProvider.GetRequiredService<AuditingInterceptor>());
        });

        services.AddScoped<IPurchaseRepository, PurchaseRepository>();

        // Exposed to other modules (Books) as a read-only contract — see
        // Summaries.SharedKernel.Contracts.Payments.
        services.AddScoped<IPurchaseVerifier, PurchaseVerifierService>();

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

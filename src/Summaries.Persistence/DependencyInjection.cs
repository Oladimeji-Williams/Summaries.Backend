using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Summaries.Application.Abstractions.Persistence;
using Summaries.Persistence.Context;
using Summaries.Persistence.Data.Interceptors;
using Summaries.Persistence.Repositories;

namespace Summaries.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found.");

        services.AddSingleton<AuditingInterceptor>();

        services.AddDbContext<ApplicationDbContext>(
            (serviceProvider, options) =>
            {
                options.UseSqlServer(connectionString);

                options.AddInterceptors(
                    serviceProvider.GetRequiredService<AuditingInterceptor>());
            });

        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IBookReadingRecordRepository, BookReadingRecordRepository>();

        return services;
    }
}
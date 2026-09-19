using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Summaries.SharedKernel.Contracts.Books;
using Summaries.Shared.Infrastructure.Auditing;
using Summaries.Modules.Books.Application.Abstractions;
using Summaries.Modules.Books.Application.CrossModule;
using Summaries.Modules.Books.Persistence;
using Summaries.Modules.Books.Persistence.Repositories;

namespace Summaries.Modules.Books;

public static class DependencyInjection
{
    public static IServiceCollection AddBooksModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        services.AddDbContext<BooksDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(connectionString, sql =>
                sql.MigrationsHistoryTable("__EFMigrationsHistory_Books"));

            options.AddInterceptors(serviceProvider.GetRequiredService<AuditingInterceptor>());
        });

        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IBookReadingRecordRepository, BookReadingRecordRepository>();

        // Exposed to other modules (Payments, Admin) as read-only contracts —
        // see Summaries.SharedKernel.Contracts.Books.
        services.AddScoped<BookCatalogService>();
        services.AddScoped<IBookCatalog>(sp => sp.GetRequiredService<BookCatalogService>());
        services.AddScoped<IBookReadingHistory>(sp => sp.GetRequiredService<BookCatalogService>());

        return services;
    }
}

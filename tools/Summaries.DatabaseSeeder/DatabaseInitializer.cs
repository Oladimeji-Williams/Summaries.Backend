using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Summaries.DatabaseSeeder.SeedData;
using Summaries.Persistence.Context;

namespace Summaries.DatabaseSeeder;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

        Console.WriteLine("Deleting database...");

        await dbContext.Database.EnsureDeletedAsync(
            cancellationToken);

        Console.WriteLine("Database deleted.");

        Console.WriteLine("Applying migrations...");

        await dbContext.Database.MigrateAsync(
            cancellationToken);

        Console.WriteLine("Migrations applied.");

        Console.WriteLine("Seeding books...");

        await BookSeedData.SeedAsync(
            dbContext,
            cancellationToken);

        Console.WriteLine("Books seeded.");
    }
}
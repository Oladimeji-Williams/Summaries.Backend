using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Summaries.DatabaseSeeder.SeedData;
using Summaries.Infrastructure.Identity;
using Summaries.Persistence.Context;

namespace Summaries.DatabaseSeeder;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var identityDbContext = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();

        Console.WriteLine("Deleting database...");
        await dbContext.Database.EnsureDeletedAsync(cancellationToken);
        Console.WriteLine("Database deleted.");

        Console.WriteLine("Applying Books migrations...");
        await dbContext.Database.MigrateAsync(cancellationToken);
        Console.WriteLine("Books migrations applied.");

        Console.WriteLine("Applying Identity migrations...");
        await identityDbContext.Database.MigrateAsync(cancellationToken);
        Console.WriteLine("Identity migrations applied.");

        Console.WriteLine("Seeding roles...");
        await IdentitySeeder.SeedAsync(scope.ServiceProvider);
        Console.WriteLine("Roles seeded.");

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        Console.WriteLine("Seeding test user...");
        var testUser = await UserSeedData.SeedAsync(userManager, cancellationToken);
        Console.WriteLine($"Test user seeded ({testUser.Email}).");

        Console.WriteLine("Seeding admin user...");
        var adminUser = await AdminUserSeedData.SeedAsync(userManager, cancellationToken);
        Console.WriteLine($"Admin user seeded ({adminUser.Email}).");

        Console.WriteLine("Seeding books...");
        await BookSeedData.SeedAsync(dbContext, cancellationToken);
        Console.WriteLine("Books seeded.");

        Console.WriteLine("Seeding reading records...");
        await BookReadingRecordSeedData.SeedAsync(dbContext, testUser.Id, cancellationToken);
        Console.WriteLine("Reading records seeded.");
    }
}
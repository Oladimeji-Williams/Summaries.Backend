using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Summaries.DatabaseSeeder.SeedData;
using Summaries.Modules.Authentication.Infrastructure.Identity;
using Summaries.Modules.Authentication.Persistence;
using Summaries.Modules.Books.Persistence;
using Summaries.Modules.Payments.Persistence;

namespace Summaries.DatabaseSeeder;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();

        var booksDbContext = scope.ServiceProvider.GetRequiredService<BooksDbContext>();
        var paymentsDbContext = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
        var identityDbContext = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();

        Console.WriteLine("Deleting databases...");
        await booksDbContext.Database.EnsureDeletedAsync(cancellationToken);
        await paymentsDbContext.Database.EnsureDeletedAsync(cancellationToken);
        await identityDbContext.Database.EnsureDeletedAsync(cancellationToken);
        Console.WriteLine("Databases deleted.");

        Console.WriteLine("Applying Books migrations...");
        await booksDbContext.Database.MigrateAsync(cancellationToken);
        Console.WriteLine("Books migrations applied.");

        Console.WriteLine("Applying Payments migrations...");
        await paymentsDbContext.Database.MigrateAsync(cancellationToken);
        Console.WriteLine("Payments migrations applied.");

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
        await BookSeedData.SeedAsync(booksDbContext, cancellationToken);
        Console.WriteLine("Books seeded.");

        Console.WriteLine("Seeding reading records...");
        await BookReadingRecordSeedData.SeedAsync(booksDbContext, testUser.Id, cancellationToken);
        Console.WriteLine("Reading records seeded.");
    }
}

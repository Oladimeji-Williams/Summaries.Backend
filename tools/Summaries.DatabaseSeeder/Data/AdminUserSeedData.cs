using Microsoft.AspNetCore.Identity;
using Summaries.Infrastructure.Identity;

namespace Summaries.DatabaseSeeder.SeedData;

public static class AdminUserSeedData
{
    public const string AdminEmail = "admin@summaries.local";
    public const string AdminPassword = "AdminPassword123!";

    public static async Task<ApplicationUser> SeedAsync(
        UserManager<ApplicationUser> userManager,
        CancellationToken cancellationToken = default)
    {
        var existing = await userManager.FindByEmailAsync(AdminEmail);
        if (existing is not null)
        {
            return existing;
        }

        var admin = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = AdminEmail,
            Email = AdminEmail,
            FirstName = "Admin",
            LastName = "User",
            CreatedAtUtc = DateTime.UtcNow,
            EmailConfirmed = true,
        };

        var result = await userManager.CreateAsync(admin, AdminPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to seed admin user: {errors}");
        }

        await userManager.AddToRoleAsync(admin, "Admin");
        await userManager.AddToRoleAsync(admin, "User");

        return admin;
    }
}
using Microsoft.AspNetCore.Identity;
using Summaries.Infrastructure.Identity;

namespace Summaries.DatabaseSeeder.SeedData;

public static class UserSeedData
{
    public const string TestUserEmail = "test@summaries.local";

    public static async Task<ApplicationUser> SeedAsync(
        UserManager<ApplicationUser> userManager,
        CancellationToken cancellationToken = default)
    {
        var existing = await userManager.FindByEmailAsync(TestUserEmail);
        if (existing is not null)
        {
            return existing;
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = TestUserEmail,
            Email = TestUserEmail,
            FirstName = "Test",
            LastName = "Reader",
            CreatedAtUtc = DateTime.UtcNow,
            EmailConfirmed = true,
        };

        var result = await userManager.CreateAsync(user, "TestPassword123!");
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to seed test user: {errors}");
        }

        await userManager.AddToRoleAsync(user, "User");

        return user;
    }
}
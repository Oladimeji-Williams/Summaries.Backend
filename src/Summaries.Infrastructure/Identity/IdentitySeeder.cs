using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Summaries.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        IServiceProvider services)
    {
        var roleManager =
            services.GetRequiredService<
                RoleManager<ApplicationRole>>();

        var userManager =
            services.GetRequiredService<
                UserManager<ApplicationUser>>();

        await SeedRoleAsync(
            roleManager,
            "User");

        await SeedRoleAsync(
            roleManager,
            "Admin");
    }

    private static async Task SeedRoleAsync(
        RoleManager<ApplicationRole> roleManager,
        string roleName)
    {
        if (await roleManager.RoleExistsAsync(roleName))
        {
            return;
        }

        var result =
            await roleManager.CreateAsync(
                new ApplicationRole
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    NormalizedName =
                        roleName.ToUpperInvariant()
                });

        if (!result.Succeeded)
        {
            var errors =
                string.Join(
                    "; ",
                    result.Errors.Select(x => x.Description));

            throw new InvalidOperationException(
                $"Failed to create role '{roleName}': {errors}");
        }
    }
}
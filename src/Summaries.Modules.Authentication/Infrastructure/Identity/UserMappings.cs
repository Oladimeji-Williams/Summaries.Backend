using Summaries.SharedKernel.Contracts.Users;

namespace Summaries.Modules.Authentication.Infrastructure.Identity;

internal static class UserMappings
{
    public static UserProfileDto ToDto(this ApplicationUser user)
    {
        return new UserProfileDto(
            user.Id,
            user.Email!,
            user.FirstName,
            user.LastName,
            user.CreatedAtUtc,
            user.AvatarUrl,
            user.PhoneNumber,
            user.Address,
            user.City,
            user.Country
        );
    }
}
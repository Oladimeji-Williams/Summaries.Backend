using Summaries.Application.Features.Users.Shared.DTOs;

namespace Summaries.Infrastructure.Identity;

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
            user.AvatarUrl);
    }
}
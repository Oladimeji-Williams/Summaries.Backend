using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.Modules.Authentication.Application.DTOs;

namespace Summaries.Modules.Authentication.Application.Mappings;

public static class AuthenticationMappings
{
    public static AuthResultDto ToDto(this AuthenticationResult result)
    {
        return new AuthResultDto(
            result.AccessToken,
            result.RefreshToken,
            result.AccessTokenExpiresAtUtc,
            result.RefreshTokenExpiresAtUtc,
            result.UserId,
            result.Email,
            result.DisplayName,
            result.Roles,
            result.AvatarUrl);
    }
}
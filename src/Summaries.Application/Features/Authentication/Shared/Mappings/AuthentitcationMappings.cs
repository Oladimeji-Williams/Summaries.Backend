using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Features.Authentication.Shared.DTOs;

namespace Summaries.Application.Features.Authentication.Shared.Mappings;

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
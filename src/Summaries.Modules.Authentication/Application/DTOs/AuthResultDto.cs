namespace Summaries.Modules.Authentication.Application.DTOs;

public sealed record AuthResultDto(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAtUtc,
    DateTime RefreshTokenExpiresAtUtc,
    Guid UserId,
    string Email,
    string DisplayName,
    IReadOnlyList<string> Roles,
    string? AvatarUrl,
    bool RequiresTwoFactor = false,
    string? TwoFactorToken = null);
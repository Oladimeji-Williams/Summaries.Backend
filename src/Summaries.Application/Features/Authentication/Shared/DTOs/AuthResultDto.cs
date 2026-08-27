namespace Summaries.Application.Features.Authentication.Shared.DTOs;

public sealed record AuthResultDto(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAtUtc,
    DateTime RefreshTokenExpiresAtUtc,
    Guid UserId,
    string Email,
    string DisplayName,
    IReadOnlyList<string> Roles);
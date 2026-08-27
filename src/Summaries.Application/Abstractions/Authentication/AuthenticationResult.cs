namespace Summaries.Application.Abstractions.Authentication;

public sealed record AuthenticationResult(
    Guid UserId,
    string Email,
    string DisplayName,
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAtUtc,
    DateTime RefreshTokenExpiresAtUtc,
    IReadOnlyList<string> Roles);
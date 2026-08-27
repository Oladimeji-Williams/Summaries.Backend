namespace Summaries.API.Contracts.Auth;

public sealed record RevokeRefreshTokenRequest(
    string RefreshToken);
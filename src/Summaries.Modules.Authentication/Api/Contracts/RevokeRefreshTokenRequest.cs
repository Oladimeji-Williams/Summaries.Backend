namespace Summaries.Modules.Authentication.Api.Contracts;

public sealed record RevokeRefreshTokenRequest(
    string RefreshToken);
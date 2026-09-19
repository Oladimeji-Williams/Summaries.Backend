namespace Summaries.Modules.Authentication.Api.Contracts;

public sealed record LoginRequest(
    string Email,
    string Password
    );
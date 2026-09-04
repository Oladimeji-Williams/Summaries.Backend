namespace Summaries.API.Contracts.Auth;

public sealed record ForgotPasswordRequest(string Email, string ResetUrlBase);
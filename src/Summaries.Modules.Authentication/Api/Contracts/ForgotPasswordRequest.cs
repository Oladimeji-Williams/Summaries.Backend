namespace Summaries.Modules.Authentication.Api.Contracts;

public sealed record ForgotPasswordRequest(string Email, string ResetUrlBase);
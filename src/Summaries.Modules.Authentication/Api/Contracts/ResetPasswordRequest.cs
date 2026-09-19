namespace Summaries.Modules.Authentication.Api.Contracts;

public sealed record ResetPasswordRequest(string Email, string Token, string NewPassword);
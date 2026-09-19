namespace Summaries.Modules.Authentication.Api.Contracts;

public sealed record RegisterRequest(string Email, string Password, string ConfirmEmailUrlBase);
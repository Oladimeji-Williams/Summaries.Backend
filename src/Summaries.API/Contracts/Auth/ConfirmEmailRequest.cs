namespace Summaries.API.Contracts.Auth;

public sealed record ConfirmEmailRequest(string Email, string Token);
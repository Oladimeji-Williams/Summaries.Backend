namespace Summaries.Modules.Authentication.Api.Contracts;

public sealed record ConfirmEmailRequest(string Email, string Token);
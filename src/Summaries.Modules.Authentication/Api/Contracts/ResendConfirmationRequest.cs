namespace Summaries.Modules.Authentication.Api.Contracts;

public sealed record ResendConfirmationRequest(string Email, string ConfirmEmailUrlBase);
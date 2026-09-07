namespace Summaries.API.Contracts.Auth;

public sealed record ResendConfirmationRequest(string Email, string ConfirmEmailUrlBase);
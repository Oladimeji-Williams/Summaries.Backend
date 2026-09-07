namespace Summaries.Application.Abstractions.Authentication;

public sealed record TwoFactorSetupResult(string SharedKey, string AuthenticatorUri);
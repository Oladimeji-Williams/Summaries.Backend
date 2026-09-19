namespace Summaries.SharedKernel.Abstractions.Authentication;

public sealed record TwoFactorSetupResult(string SharedKey, string AuthenticatorUri);
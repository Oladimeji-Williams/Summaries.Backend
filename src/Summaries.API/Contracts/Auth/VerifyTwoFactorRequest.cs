namespace Summaries.API.Contracts.Auth;

public sealed record VerifyTwoFactorRequest(string TwoFactorToken, string Code);
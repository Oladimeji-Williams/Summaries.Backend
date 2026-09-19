namespace Summaries.Modules.Authentication.Api.Contracts;

public sealed record VerifyTwoFactorRequest(string TwoFactorToken, string Code);
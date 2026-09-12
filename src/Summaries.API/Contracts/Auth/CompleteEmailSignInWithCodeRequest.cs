namespace Summaries.API.Contracts.Auth;
public sealed record CompleteEmailSignInWithCodeRequest(string Email, string Code);
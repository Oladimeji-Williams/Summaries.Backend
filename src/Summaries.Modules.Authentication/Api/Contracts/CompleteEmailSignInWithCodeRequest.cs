namespace Summaries.Modules.Authentication.Api.Contracts;
public sealed record CompleteEmailSignInWithCodeRequest(string Email, string Code);
namespace Summaries.Application.Abstractions.Authentication;

public abstract record LoginOutcome
{
    public sealed record Success(AuthenticationResult Result) : LoginOutcome;
    public sealed record RequiresTwoFactor(string TwoFactorToken) : LoginOutcome;
    public sealed record InvalidCredentials : LoginOutcome;
    public sealed record EmailNotConfirmed : LoginOutcome;
    public sealed record AccountLockedOut(DateTimeOffset? LockoutEndUtc) : LoginOutcome;   // add this
}
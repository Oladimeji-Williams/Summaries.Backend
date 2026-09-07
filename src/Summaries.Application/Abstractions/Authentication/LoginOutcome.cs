namespace Summaries.Application.Abstractions.Authentication;

public abstract record LoginOutcome
{
    private LoginOutcome() { }

    public sealed record Success(AuthenticationResult Result) : LoginOutcome;
    public sealed record InvalidCredentials : LoginOutcome;
    public sealed record EmailNotConfirmed : LoginOutcome;
    public sealed record RequiresTwoFactor(string TwoFactorToken) : LoginOutcome;
}
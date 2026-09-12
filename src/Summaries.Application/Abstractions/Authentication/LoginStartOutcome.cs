namespace Summaries.Application.Abstractions.Authentication;

public abstract record LoginStartOutcome
{
    public sealed record AccountNotFound : LoginStartOutcome;
    public sealed record UsePassword : LoginStartOutcome;
    public sealed record EmailCodeSent : LoginStartOutcome;
    public sealed record NoPasswordSet : LoginStartOutcome;
    public sealed record EmailDeliveryFailed : LoginStartOutcome;
}
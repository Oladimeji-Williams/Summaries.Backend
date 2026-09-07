using FluentValidation;

namespace Summaries.Application.Features.Authentication.Commands.VerifyTwoFactor;

public sealed class VerifyTwoFactorCommandValidator : AbstractValidator<VerifyTwoFactorCommand>
{
    public VerifyTwoFactorCommandValidator()
    {
        RuleFor(x => x.TwoFactorToken).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().Length(6);
    }
}
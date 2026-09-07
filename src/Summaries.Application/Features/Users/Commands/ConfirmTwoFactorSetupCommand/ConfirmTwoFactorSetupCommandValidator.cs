using FluentValidation;

namespace Summaries.Application.Features.Users.Commands.EnableTwoFactor;

public sealed class ConfirmTwoFactorSetupCommandValidator : AbstractValidator<ConfirmTwoFactorSetupCommand>
{
    public ConfirmTwoFactorSetupCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().Length(6);
    }
}
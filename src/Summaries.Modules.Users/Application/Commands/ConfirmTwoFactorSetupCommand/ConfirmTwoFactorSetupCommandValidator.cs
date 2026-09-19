using FluentValidation;

namespace Summaries.Modules.Users.Application.Commands.ConfirmTwoFactorSetupCommand;

public sealed class ConfirmTwoFactorSetupCommandValidator : AbstractValidator<ConfirmTwoFactorSetupCommand>
{
    public ConfirmTwoFactorSetupCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().Length(6);
    }
}
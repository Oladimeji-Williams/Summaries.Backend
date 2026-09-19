using FluentValidation;

namespace Summaries.Modules.Authentication.Application.Commands.ForgotPasswordCommand;

public sealed class ForgotPasswordCommandValidator
    : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.ResetUrlBase)
            .NotEmpty()
            .MaximumLength(2048);
    }
}
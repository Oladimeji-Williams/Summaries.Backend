using FluentValidation;

namespace Summaries.Application.Features.Authentication.Commands.ForgotPassword;

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
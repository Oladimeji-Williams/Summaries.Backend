using FluentValidation;

namespace Summaries.Modules.Authentication.Application.Commands.ResendEmailConfirmationCommand;

public sealed class ResendEmailConfirmationCommandValidator : AbstractValidator<ResendEmailConfirmationCommand>
{
    public ResendEmailConfirmationCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.ConfirmEmailUrlBase).NotEmpty().MaximumLength(2048);
    }
}
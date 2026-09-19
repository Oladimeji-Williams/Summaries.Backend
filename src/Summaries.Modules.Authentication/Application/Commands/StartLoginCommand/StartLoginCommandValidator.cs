using FluentValidation;

namespace Summaries.Modules.Authentication.Application.Commands.StartLoginCommand;

public sealed class StartLoginCommandValidator : AbstractValidator<StartLoginCommand>
{
    public StartLoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
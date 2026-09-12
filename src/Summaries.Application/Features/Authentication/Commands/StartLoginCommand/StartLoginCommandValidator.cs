using FluentValidation;

namespace Summaries.Application.Features.Authentication.Commands.StartLoginCommand;

public sealed class StartLoginCommandValidator : AbstractValidator<StartLoginCommand>
{
    public StartLoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
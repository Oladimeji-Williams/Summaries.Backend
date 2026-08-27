using FluentValidation;

namespace Summaries.Application.Features.Authentication.Commands.RefreshTokenCommand;

public sealed class RefreshTokenCommandValidator
    : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .MaximumLength(1000);
    }
}
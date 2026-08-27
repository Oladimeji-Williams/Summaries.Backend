using FluentValidation;

namespace Summaries.Application.Features.Authentication.Commands.RevokeRefreshTokenCommand;

public sealed class RevokeRefreshTokenCommandValidator
    : AbstractValidator<RevokeRefreshTokenCommand>
{
    public RevokeRefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .MaximumLength(1000);
    }
}
using FluentValidation;

namespace Summaries.Application.Features.Books.Commands.MarkBookAsReadCommand;

public sealed class MarkBookAsReadCommandValidator : AbstractValidator<MarkBookAsReadCommand>
{
    public MarkBookAsReadCommandValidator()
    {
        RuleFor(x => x.Rating).InclusiveBetween(0m, 5m).When(x => x.Rating.HasValue);
    }
}
using FluentValidation;

namespace Summaries.Modules.Books.Application.Commands.MarkBookAsRead;

public sealed class MarkBookAsReadCommandValidator : AbstractValidator<MarkBookAsReadCommand>
{
    public MarkBookAsReadCommandValidator()
    {
        RuleFor(x => x.Rating).InclusiveBetween(0m, 5m).When(x => x.Rating.HasValue);
    }
}
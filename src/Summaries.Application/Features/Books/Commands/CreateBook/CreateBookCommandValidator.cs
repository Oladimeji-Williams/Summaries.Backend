using FluentValidation;

namespace Summaries.Application.Features.Books.Commands.CreateBookCommand;

public sealed class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Author).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Isbn).MaximumLength(20);
        RuleFor(x => x.Publisher).MaximumLength(200);
        RuleFor(x => x.Genre).MaximumLength(100);
        RuleFor(x => x.PublishedYear).InclusiveBetween(1000, DateTime.UtcNow.Year).When(x => x.PublishedYear.HasValue);
        RuleFor(x => x.PageCount).GreaterThan(0).When(x => x.PageCount.HasValue);
    }
}
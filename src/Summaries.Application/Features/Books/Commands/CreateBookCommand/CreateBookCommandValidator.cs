using FluentValidation;

namespace Summaries.Application.Features.Books.Commands.CreateBookCommand;

public sealed class CreateBookCommandValidator
    : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Author)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Rating)
            .InclusiveBetween(0m, 1m)
            .When(x => x.Rating.HasValue);
    }
}
using FluentValidation;

namespace Summaries.Application.Features.Books.Commands.UpdateBookCommand;

public sealed class UpdateBookCommandValidator
    : AbstractValidator<UpdateBookCommand>
{
    public UpdateBookCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Author)
            .NotEmpty()
            .MaximumLength(200);
    }
}
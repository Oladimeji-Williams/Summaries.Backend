using FluentValidation;

namespace Summaries.Modules.Books.Application.Commands.UpdateBookPriceCommand;

public sealed class UpdateBookPriceCommandValidator : AbstractValidator<UpdateBookPriceCommand>
{
    public UpdateBookPriceCommandValidator()
    {
        RuleFor(x => x.PriceKobo).GreaterThanOrEqualTo(0).When(x => x.PriceKobo.HasValue);
    }
}
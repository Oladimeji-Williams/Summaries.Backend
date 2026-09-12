using MediatR;
using Summaries.Application.Abstractions.Persistence;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Books.Shared.Errors;

namespace Summaries.Application.Features.Books.Commands.UpdateBookPriceCommand;

public sealed class UpdateBookPriceCommandHandler(IBookRepository bookRepository)
    : IRequestHandler<UpdateBookPriceCommand, Result>
{
    public async Task<Result> Handle(UpdateBookPriceCommand request, CancellationToken cancellationToken)
    {
        var book = await bookRepository.GetByIdAsync(request.BookId, cancellationToken);
        if (book is null)
        {
            return Result.Failure(BookErrors.NotFound(request.BookId));
        }

        book.SetPrice(request.PriceKobo);
        await bookRepository.UpdateAsync(book, cancellationToken);

        return Result.Success();
    }
}
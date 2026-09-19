using MediatR;
using Summaries.Modules.Books.Application.Abstractions;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.SharedKernel.Contracts.Books;

namespace Summaries.Modules.Books.Application.Commands.UpdateBookPriceCommand;

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
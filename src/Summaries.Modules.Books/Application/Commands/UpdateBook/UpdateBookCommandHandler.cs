using MediatR;
using Summaries.Modules.Books.Application.Abstractions;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.SharedKernel.Contracts.Books;

namespace Summaries.Modules.Books.Application.Commands.UpdateBook;

public sealed class UpdateBookCommandHandler(IBookRepository bookRepository)
    : IRequestHandler<UpdateBookCommand, Result>
{
    public async Task<Result> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var book = await bookRepository.GetByIdAsync(request.Id, cancellationToken);
        if (book is null)
        {
            return Result.Failure(BookErrors.NotFound(request.Id));
        }

        var existingBook = await bookRepository.GetByTitleAsync(request.Title, cancellationToken);
        if (existingBook is not null && existingBook.Id != request.Id)
        {
            return Result.Failure(BookErrors.AlreadyExists(request.Title));
        }

        book.Update(
            request.Title,
            request.Author,
            request.Description,
            request.Isbn,
            request.Publisher,
            request.PublishedYear,
            request.Genre,
            request.PageCount);

        await bookRepository.UpdateAsync(book, cancellationToken);

        return Result.Success();
    }
}
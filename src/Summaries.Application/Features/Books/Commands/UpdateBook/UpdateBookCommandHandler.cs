using MediatR;
using Summaries.Application.Abstractions.Persistence;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Books.Shared.Errors;

namespace Summaries.Application.Features.Books.Commands.UpdateBookCommand;

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

        book.Update(request.Title, request.Author, request.Description);
        await bookRepository.UpdateAsync(book, cancellationToken);

        return Result.Success();
    }
}
using MediatR;
using Summaries.Application.Abstractions.Persistence;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Books.Shared.Errors;
using Summaries.Domain.Enums;

namespace Summaries.Application.Features.Books.Commands.StartReadingBookCommand;

public sealed class StartReadingBookCommandHandler(
    IBookRepository bookRepository)
    : IRequestHandler<StartReadingBookCommand, Result>
{
    private readonly IBookRepository _bookRepository = bookRepository;

    public async Task<Result> Handle(
        StartReadingBookCommand request,
        CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(
            request.BookId,
            cancellationToken);
        if (book is null)
        {
            return Result.Failure(
                BookErrors.NotFound(request.BookId));
        }
        if (book.Status != BookStatus.NotStarted)
        {
            return Result.Failure(
                BookErrors.NotStarted());
        }
        book.StartReading(DateTimeOffset.UtcNow);
        await _bookRepository.UpdateAsync(
            book,
            cancellationToken);
        return Result.Success();
    }
}
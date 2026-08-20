using MediatR;
using Summaries.Application.Abstractions.Persistence;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Books.Shared.Errors;
using Summaries.Domain.Enums;

namespace Summaries.Application.Features.Books.Commands.MarkBookAsReadCommand;

public sealed class MarkBookAsReadCommandHandler(
    IBookRepository bookRepository)
    : IRequestHandler<MarkBookAsReadCommand, Result>
{
    private readonly IBookRepository _bookRepository = bookRepository;

    public async Task<Result> Handle(
        MarkBookAsReadCommand request,
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
        if (book.Status != BookStatus.InProgress)
        {
            return Result.Failure(
                BookErrors.NotInProgress());
        }
        book.MarkAsRead(DateTimeOffset.UtcNow);
        await _bookRepository.UpdateAsync(
            book,
            cancellationToken);
        return Result.Success();
    }
}
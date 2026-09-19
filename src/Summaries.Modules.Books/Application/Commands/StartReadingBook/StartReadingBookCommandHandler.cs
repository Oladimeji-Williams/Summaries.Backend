using MediatR;
using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.Modules.Books.Application.Abstractions;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.SharedKernel.Contracts.Books;
using Summaries.SharedKernel.Contracts.Users;

using Summaries.Modules.Books.Domain.Entities;
namespace Summaries.Modules.Books.Application.Commands.StartReadingBook;

public sealed class StartReadingBookCommandHandler(
    IBookRepository bookRepository,
    IBookReadingRecordRepository readingRecordRepository,
    ICurrentUser currentUser)
    : IRequestHandler<StartReadingBookCommand, Result>
{
    public async Task<Result> Handle(StartReadingBookCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Result.Failure(UserErrors.NotAuthenticated());
        }

        var book = await bookRepository.GetByIdAsync(request.BookId, cancellationToken);
        if (book is null)
        {
            return Result.Failure(BookErrors.NotFound(request.BookId));
        }

        var record = await readingRecordRepository.GetByUserAndBookAsync(
            currentUser.UserId.Value, request.BookId, cancellationToken);

        if (record is null)
        {
            record = new BookReadingRecord(request.BookId, currentUser.UserId.Value);
            record.StartReading(DateTimeOffset.UtcNow);
            await readingRecordRepository.AddAsync(record, cancellationToken);
            return Result.Success();
        }

        if (record.Status != BookStatus.NotStarted)
        {
            return Result.Failure(BookErrors.NotStarted());
        }

        record.StartReading(DateTimeOffset.UtcNow);
        await readingRecordRepository.UpdateAsync(record, cancellationToken);
        return Result.Success();
    }
}
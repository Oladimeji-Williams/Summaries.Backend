using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Abstractions.Persistence;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Books.Shared.Errors;
using Summaries.Application.Features.Users.Shared.Errors;
using Summaries.Domain.Entities;
using Summaries.Domain.Enums;

namespace Summaries.Application.Features.Books.Commands.StartReadingBookCommand;

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
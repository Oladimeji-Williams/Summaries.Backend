using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Abstractions.Persistence;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Books.Shared.Errors;
using Summaries.Application.Features.Users.Shared.Errors;
using Summaries.Domain.Enums;

namespace Summaries.Application.Features.Books.Commands.MarkBookAsReadCommand;

public sealed class MarkBookAsReadCommandHandler(
    IBookReadingRecordRepository readingRecordRepository,
    ICurrentUser currentUser)
    : IRequestHandler<MarkBookAsReadCommand, Result>
{
    public async Task<Result> Handle(MarkBookAsReadCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Result.Failure(UserErrors.NotAuthenticated());
        }

        var record = await readingRecordRepository.GetByUserAndBookAsync(
            currentUser.UserId.Value, request.BookId, cancellationToken);

        if (record is null || record.Status != BookStatus.InProgress)
        {
            return Result.Failure(BookErrors.NotInProgress());
        }

        record.MarkAsRead(DateTimeOffset.UtcNow, request.Rating);
        await readingRecordRepository.UpdateAsync(record, cancellationToken);

        return Result.Success();
    }
}
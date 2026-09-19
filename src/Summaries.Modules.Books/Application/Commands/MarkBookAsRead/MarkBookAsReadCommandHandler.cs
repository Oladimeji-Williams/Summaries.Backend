using MediatR;
using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.Modules.Books.Application.Abstractions;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.SharedKernel.Contracts.Books;
using Summaries.SharedKernel.Contracts.Users;

namespace Summaries.Modules.Books.Application.Commands.MarkBookAsRead;

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
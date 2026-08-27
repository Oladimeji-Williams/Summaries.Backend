using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Abstractions.Persistence;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Admin.Shared.DTOs;
using Summaries.Application.Features.Users.Shared.Errors;

namespace Summaries.Application.Features.Admin.Queries.GetUserReadingHistory;

public sealed class GetUserReadingHistoryQueryHandler(
    IIdentityService identityService,
    IBookReadingRecordRepository readingRecordRepository,
    IBookRepository bookRepository)
    : IRequestHandler<GetUserReadingHistoryQuery, Result<UserReadingHistoryDto>>
{
    public async Task<Result<UserReadingHistoryDto>> Handle(
        GetUserReadingHistoryQuery request, CancellationToken cancellationToken)
    {
        var profile = await identityService.GetProfileAsync(request.UserId, cancellationToken);
        if (profile is null)
        {
            return Result<UserReadingHistoryDto>.Failure(UserErrors.NotFound(request.UserId));
        }

        var records = await readingRecordRepository.GetAllForUserAsync(request.UserId, cancellationToken);
        var books = await bookRepository.GetAllAsync(cancellationToken);
        var booksById = books.ToDictionary(b => b.Id);

        var entries = records
            .Where(r => booksById.ContainsKey(r.BookId))
            .Select(r =>
            {
                var book = booksById[r.BookId];
                return new BookReadingEntryDto(
                    book.Id, book.Title, book.Author, r.Status, r.Rating, r.DateStarted, r.DateRead);
            })
            .ToList();

        return Result<UserReadingHistoryDto>.Success(
            new UserReadingHistoryDto(profile.Id, profile.Email, profile.FirstName, profile.LastName, entries));
    }
}
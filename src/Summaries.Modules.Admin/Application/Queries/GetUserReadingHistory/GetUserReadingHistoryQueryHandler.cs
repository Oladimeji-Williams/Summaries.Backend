using MediatR;
using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.SharedKernel.Contracts.Books;
using Summaries.SharedKernel.Contracts.Users;
using Summaries.Modules.Admin.Application.DTOs;

namespace Summaries.Modules.Admin.Application.Queries.GetUserReadingHistory;

public sealed class GetUserReadingHistoryQueryHandler(
    IIdentityService identityService,
    IBookReadingHistory readingHistory,
    IBookCatalog bookCatalog)
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

        var records = await readingHistory.GetAllForUserAsync(request.UserId, cancellationToken);
        var books = await bookCatalog.GetAllAsync(cancellationToken);
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

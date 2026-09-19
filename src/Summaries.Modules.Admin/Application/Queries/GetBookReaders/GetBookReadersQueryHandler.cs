using MediatR;
using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.SharedKernel.Contracts.Books;
using Summaries.Modules.Admin.Application.DTOs;

namespace Summaries.Modules.Admin.Application.Queries.GetBookReaders;

public sealed class GetBookReadersQueryHandler(
    IBookCatalog bookCatalog,
    IBookReadingHistory readingHistory,
    IIdentityService identityService)
    : IRequestHandler<GetBookReadersQuery, Result<BookReadersDto>>
{
    public async Task<Result<BookReadersDto>> Handle(
        GetBookReadersQuery request, CancellationToken cancellationToken)
    {
        var book = await bookCatalog.GetByIdAsync(request.BookId, cancellationToken);
        if (book is null)
        {
            return Result<BookReadersDto>.Failure(BookErrors.NotFound(request.BookId));
        }

        var records = await readingHistory.GetAllForBookAsync(request.BookId, cancellationToken);
        var userIds = records.Select(r => r.UserId).Distinct();
        var usersById = await identityService.GetUsersByIdsAsync(userIds, cancellationToken);

        var readers = records
            .Where(r => usersById.ContainsKey(r.UserId))
            .Select(r =>
            {
                var user = usersById[r.UserId];
                return new ReaderEntryDto(
                    user.Id, user.Email, user.FirstName, user.LastName, r.Status, r.Rating, r.DateStarted, r.DateRead);
            })
            .ToList();

        return Result<BookReadersDto>.Success(
            new BookReadersDto(book.Id, book.Title, book.Author, readers));
    }
}

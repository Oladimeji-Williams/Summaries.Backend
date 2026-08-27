using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Abstractions.Persistence;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Admin.Shared.DTOs;
using Summaries.Application.Features.Books.Shared.Errors;

namespace Summaries.Application.Features.Admin.Queries.GetBookReaders;

public sealed class GetBookReadersQueryHandler(
    IBookRepository bookRepository,
    IBookReadingRecordRepository readingRecordRepository,
    IIdentityService identityService)
    : IRequestHandler<GetBookReadersQuery, Result<BookReadersDto>>
{
    public async Task<Result<BookReadersDto>> Handle(
        GetBookReadersQuery request, CancellationToken cancellationToken)
    {
        var book = await bookRepository.GetByIdAsync(request.BookId, cancellationToken);
        if (book is null)
        {
            return Result<BookReadersDto>.Failure(BookErrors.NotFound(request.BookId));
        }

        var records = await readingRecordRepository.GetAllForBookAsync(request.BookId, cancellationToken);
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
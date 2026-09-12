using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Abstractions.Persistence;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Books.Shared.DTOs;
using Summaries.Application.Features.Books.Shared.Mappings;
using Summaries.Domain.Entities;

namespace Summaries.Application.Features.Books.Queries.GetAllBooksQuery;

public sealed class GetAllBooksQueryHandler(
    IBookRepository bookRepository,
    IBookReadingRecordRepository readingRecordRepository,
    IPurchaseRepository purchaseRepository,
    ICurrentUser currentUser)
    : IRequestHandler<GetAllBooksQuery, Result<IReadOnlyList<BookDto>>>
{
    public async Task<Result<IReadOnlyList<BookDto>>> Handle(
        GetAllBooksQuery request, CancellationToken cancellationToken)
    {
        var books = await bookRepository.GetAllAsync(cancellationToken);

        var recordsByBookId = currentUser.UserId is null
            ? new Dictionary<int, BookReadingRecord>()
            : (await readingRecordRepository.GetAllForUserAsync(currentUser.UserId.Value, cancellationToken))
                .ToDictionary(r => r.BookId);

        var purchasedBookIds = currentUser.UserId is null
            ? new HashSet<int>()
            : (await purchaseRepository.GetSuccessfulBookIdsForUserAsync(currentUser.UserId.Value, cancellationToken))
                .ToHashSet();

        var dtos = books
            .Select(book => book.ToDto(
                recordsByBookId.GetValueOrDefault(book.Id),
                purchasedBookIds.Contains(book.Id)))
            .ToList();

        return Result<IReadOnlyList<BookDto>>.Success(dtos);
    }
}
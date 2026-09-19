using MediatR;
using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.SharedKernel.Contracts.Payments;
using Summaries.Modules.Books.Application.Abstractions;
using Summaries.Modules.Books.Application.DTOs;
using Summaries.Modules.Books.Application.Mappings;
using Summaries.Modules.Books.Domain.Entities;
namespace Summaries.Modules.Books.Application.Queries.GetAllBooks;

public sealed class GetAllBooksQueryHandler(
    IBookRepository bookRepository,
    IBookReadingRecordRepository readingRecordRepository,
    IPurchaseVerifier purchaseVerifier,
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
            : (await purchaseVerifier.GetPurchasedBookIdsAsync(currentUser.UserId.Value, cancellationToken))
                .ToHashSet();

        var dtos = books
            .Select(book => book.ToDto(
                recordsByBookId.GetValueOrDefault(book.Id),
                purchasedBookIds.Contains(book.Id)))
            .ToList();

        return Result<IReadOnlyList<BookDto>>.Success(dtos);
    }
}
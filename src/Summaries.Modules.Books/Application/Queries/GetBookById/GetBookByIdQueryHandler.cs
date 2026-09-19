using MediatR;
using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.SharedKernel.Contracts.Books;
using Summaries.SharedKernel.Contracts.Payments;
using Summaries.Modules.Books.Application.Abstractions;
using Summaries.Modules.Books.Application.DTOs;
using Summaries.Modules.Books.Application.Mappings;

namespace Summaries.Modules.Books.Application.Queries.GetBookById;

public sealed class GetBookByIdQueryHandler(
    IBookRepository bookRepository,
    IBookReadingRecordRepository readingRecordRepository,
    IPurchaseVerifier purchaseVerifier,
    ICurrentUser currentUser)
    : IRequestHandler<GetBookByIdQuery, Result<BookDto>>
{
    public async Task<Result<BookDto>> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var book = await bookRepository.GetByIdAsync(request.Id, cancellationToken);
        if (book is null)
        {
            return Result<BookDto>.Failure(BookErrors.NotFound(request.Id));
        }

        var record = currentUser.UserId is null
            ? null
            : await readingRecordRepository.GetByUserAndBookAsync(currentUser.UserId.Value, request.Id, cancellationToken);

        var isPurchased = currentUser.UserId is not null &&
            await purchaseVerifier.HasSuccessfulPurchaseAsync(currentUser.UserId.Value, request.Id, cancellationToken);

        return Result<BookDto>.Success(book.ToDto(record, isPurchased));
    }
}
using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Abstractions.Persistence;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Books.Shared.DTOs;
using Summaries.Application.Features.Books.Shared.Errors;
using Summaries.Application.Features.Books.Shared.Mappings;

namespace Summaries.Application.Features.Books.Queries.GetBookByIdQuery;

public sealed class GetBookByIdQueryHandler(
    IBookRepository bookRepository,
    IBookReadingRecordRepository readingRecordRepository,
    IPurchaseRepository purchaseRepository,
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
            await purchaseRepository.GetSuccessfulForUserAndBookAsync(currentUser.UserId.Value, request.Id, cancellationToken) is not null;

        return Result<BookDto>.Success(book.ToDto(record, isPurchased));
    }
}
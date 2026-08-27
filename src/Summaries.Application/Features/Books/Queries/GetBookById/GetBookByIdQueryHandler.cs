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

        return Result<BookDto>.Success(book.ToDto(record));
    }
}
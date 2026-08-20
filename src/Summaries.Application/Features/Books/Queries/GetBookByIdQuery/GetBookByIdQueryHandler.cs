using MediatR;
using Summaries.Application.Abstractions.Persistence;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Books.Shared.DTOs;
using Summaries.Application.Features.Books.Shared.Errors;
using Summaries.Application.Features.Books.Shared.Mappings;

namespace Summaries.Application.Features.Books.Queries.GetBookByIdQuery;

public sealed class GetBookByIdQueryHandler(
    IBookRepository bookRepository)
    : IRequestHandler<GetBookByIdQuery, Result<BookDto>>
{
    private readonly IBookRepository _bookRepository = bookRepository;

    public async Task<Result<BookDto>> Handle(
        GetBookByIdQuery request,
        CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (book is null)
        {
            return Result<BookDto>.Failure(
                BookErrors.NotFound(request.Id));
        }

        return Result<BookDto>.Success(
            book.ToDto());
    }
}
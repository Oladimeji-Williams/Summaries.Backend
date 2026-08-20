using MediatR;
using Summaries.Application.Abstractions.Persistence;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Books.Shared.DTOs;
using Summaries.Application.Features.Books.Shared.Mappings;

namespace Summaries.Application.Features.Books.Queries.GetAllBooksQuery;

public sealed class GetAllBooksQueryHandler(
    IBookRepository bookRepository)
    : IRequestHandler<GetAllBooksQuery, Result<IReadOnlyList<BookDto>>>
{
    private readonly IBookRepository _bookRepository = bookRepository;

    public async Task<Result<IReadOnlyList<BookDto>>> Handle(
        GetAllBooksQuery request,
        CancellationToken cancellationToken)
    {
        var books = await _bookRepository.GetAllAsync(
            cancellationToken);

        return Result<IReadOnlyList<BookDto>>.Success(
            books.ToDto());
    }
}
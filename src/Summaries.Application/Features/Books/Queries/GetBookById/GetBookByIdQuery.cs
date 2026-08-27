using MediatR;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Books.Shared.DTOs;

namespace Summaries.Application.Features.Books.Queries.GetBookByIdQuery;

public sealed record GetBookByIdQuery(
    int Id
) : IRequest<Result<BookDto>>;
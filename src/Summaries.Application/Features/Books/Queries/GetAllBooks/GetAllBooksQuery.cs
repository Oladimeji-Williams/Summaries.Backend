using MediatR;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Books.Shared.DTOs;

namespace Summaries.Application.Features.Books.Queries.GetAllBooksQuery;

public sealed record GetAllBooksQuery
    : IRequest<Result<IReadOnlyList<BookDto>>>;
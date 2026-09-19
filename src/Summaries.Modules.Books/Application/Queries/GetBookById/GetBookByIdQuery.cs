using MediatR;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.Modules.Books.Application.DTOs;

namespace Summaries.Modules.Books.Application.Queries.GetBookById;

public sealed record GetBookByIdQuery(
    int Id
) : IRequest<Result<BookDto>>;
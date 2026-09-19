using MediatR;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.Modules.Books.Application.DTOs;

namespace Summaries.Modules.Books.Application.Queries.GetAllBooks;

public sealed record GetAllBooksQuery
    : IRequest<Result<IReadOnlyList<BookDto>>>;
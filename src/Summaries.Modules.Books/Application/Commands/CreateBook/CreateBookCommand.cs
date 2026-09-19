using MediatR;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.Modules.Books.Application.DTOs;

namespace Summaries.Modules.Books.Application.Commands.CreateBook;

public sealed record CreateBookCommand(
    string Title,
    string Author,
    string Description,
    string? Isbn,
    string? Publisher,
    int? PublishedYear,
    string? Genre,
    int? PageCount) : IRequest<Result<BookDto>>;
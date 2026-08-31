using MediatR;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Books.Shared.DTOs;

namespace Summaries.Application.Features.Books.Commands.CreateBookCommand;

public sealed record CreateBookCommand(
    string Title,
    string Author,
    string Description,
    string? Isbn,
    string? Publisher,
    int? PublishedYear,
    string? Genre,
    int? PageCount) : IRequest<Result<BookDto>>;
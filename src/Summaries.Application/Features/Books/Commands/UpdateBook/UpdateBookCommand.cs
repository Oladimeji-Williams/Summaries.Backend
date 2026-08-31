using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Books.Commands.UpdateBookCommand;

public sealed record UpdateBookCommand(
    int Id,
    string Title,
    string Author,
    string Description,
    string? Isbn,
    string? Publisher,
    int? PublishedYear,
    string? Genre,
    int? PageCount) : IRequest<Result>;
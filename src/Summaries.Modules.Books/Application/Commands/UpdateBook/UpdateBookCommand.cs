using MediatR;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Books.Application.Commands.UpdateBook;

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
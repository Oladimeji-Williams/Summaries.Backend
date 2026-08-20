using MediatR;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Books.Shared.DTOs;

namespace Summaries.Application.Features.Books.Commands.CreateBookCommand;

public sealed record CreateBookCommand(
    string Title,
    string Author,
    string Description,
    decimal? Rating) : IRequest<Result<BookDto>>;
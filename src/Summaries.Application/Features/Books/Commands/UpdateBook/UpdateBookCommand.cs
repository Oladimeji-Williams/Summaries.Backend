using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Books.Commands.UpdateBookCommand;

public sealed record UpdateBookCommand(
    int Id,
    string Title,
    string Author,
    string Description) : IRequest<Result>;
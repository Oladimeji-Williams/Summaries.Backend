using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Books.Commands.StartReadingBookCommand;

public sealed record StartReadingBookCommand(
    int BookId) : IRequest<Result>;
using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Books.Commands.DeleteBookCommand;

public sealed record DeleteBookCommand(
    int Id
) : IRequest<Result>;
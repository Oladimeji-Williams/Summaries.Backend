using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Books.Commands.MarkBookAsReadCommand;

public sealed record MarkBookAsReadCommand(
    int BookId) : IRequest<Result>;
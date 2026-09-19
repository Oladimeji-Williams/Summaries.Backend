using MediatR;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Books.Application.Commands.MarkBookAsRead;

public sealed record MarkBookAsReadCommand(int BookId, decimal? Rating) : IRequest<Result>;
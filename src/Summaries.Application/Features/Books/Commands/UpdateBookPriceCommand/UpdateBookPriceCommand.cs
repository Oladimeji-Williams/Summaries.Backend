using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Books.Commands.UpdateBookPriceCommand;

public sealed record UpdateBookPriceCommand(int BookId, long? PriceKobo) : IRequest<Result>;
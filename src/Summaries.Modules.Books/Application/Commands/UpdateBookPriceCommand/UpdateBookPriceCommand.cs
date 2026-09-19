using MediatR;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Books.Application.Commands.UpdateBookPriceCommand;

public sealed record UpdateBookPriceCommand(int BookId, long? PriceKobo) : IRequest<Result>;
using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Books.Commands.VerifyPurchaseCommand;

public sealed record VerifyPurchaseCommand(string Reference) : IRequest<Result<bool>>;
using MediatR;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Payments.Application.Commands.VerifyPurchaseCommand;

public sealed record VerifyPurchaseCommand(string Reference) : IRequest<Result<bool>>;
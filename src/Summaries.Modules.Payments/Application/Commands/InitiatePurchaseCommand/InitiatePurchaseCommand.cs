using MediatR;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Payments.Application.Commands.InitiatePurchaseCommand;

public sealed record InitiatePurchaseCommand(int BookId) : IRequest<Result<InitiatePurchaseResultDto>>;

public sealed record InitiatePurchaseResultDto(string AuthorizationUrl, string Reference);
using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Books.Commands.InitiatePurchaseCommand;

public sealed record InitiatePurchaseCommand(int BookId) : IRequest<Result<InitiatePurchaseResultDto>>;

public sealed record InitiatePurchaseResultDto(string AuthorizationUrl, string Reference);
using MediatR;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Authentication.Application.Commands.RevokeRefreshTokenCommand;

public sealed record RevokeRefreshTokenCommand(
    string RefreshToken)
    : IRequest<Result>;
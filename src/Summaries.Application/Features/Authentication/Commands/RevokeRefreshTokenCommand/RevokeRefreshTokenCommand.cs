using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Authentication.Commands.RevokeRefreshTokenCommand;

public sealed record RevokeRefreshTokenCommand(
    string RefreshToken)
    : IRequest<Result>;
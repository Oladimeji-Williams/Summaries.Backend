using MediatR;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Authentication.Shared.DTOs;

namespace Summaries.Application.Features.Authentication.Commands.RefreshTokenCommand;

public sealed record RefreshTokenCommand(
    string RefreshToken)
    : IRequest<Result<AuthResultDto>>;
using MediatR;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.Modules.Authentication.Application.DTOs;

namespace Summaries.Modules.Authentication.Application.Commands.RefreshTokenCommand;

public sealed record RefreshTokenCommand(
    string RefreshToken)
    : IRequest<Result<AuthResultDto>>;
using MediatR;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Authentication.Shared.DTOs;

namespace Summaries.Application.Features.Authentication.Commands.LoginCommand;

public sealed record LoginCommand(
    string Email,
    string Password)
    : IRequest<Result<AuthResultDto>>;
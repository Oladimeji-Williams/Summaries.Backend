using MediatR;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.Modules.Authentication.Application.DTOs;

namespace Summaries.Modules.Authentication.Application.Commands.LoginCommand;

public sealed record LoginCommand(
    string Email,
    string Password)
    : IRequest<Result<AuthResultDto>>;
using MediatR;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.Modules.Authentication.Application.DTOs;

namespace Summaries.Modules.Authentication.Application.Commands.VerifyTwoFactorCommand;

public sealed record VerifyTwoFactorCommand(string TwoFactorToken, string Code) : IRequest<Result<AuthResultDto>>;
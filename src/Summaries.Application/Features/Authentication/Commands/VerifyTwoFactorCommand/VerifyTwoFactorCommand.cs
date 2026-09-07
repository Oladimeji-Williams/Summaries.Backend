using MediatR;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Authentication.Shared.DTOs;

namespace Summaries.Application.Features.Authentication.Commands.VerifyTwoFactor;

public sealed record VerifyTwoFactorCommand(string TwoFactorToken, string Code) : IRequest<Result<AuthResultDto>>;
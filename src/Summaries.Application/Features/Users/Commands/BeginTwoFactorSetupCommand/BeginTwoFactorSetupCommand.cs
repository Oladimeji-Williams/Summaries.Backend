using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Users.Commands.EnableTwoFactor;

public sealed record BeginTwoFactorSetupCommand : IRequest<Result<TwoFactorSetupResult>>;
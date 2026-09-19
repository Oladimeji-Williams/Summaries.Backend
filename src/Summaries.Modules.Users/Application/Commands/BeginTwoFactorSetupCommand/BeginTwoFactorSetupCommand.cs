using MediatR;
using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Users.Application.Commands.BeginTwoFactorSetupCommand;

public sealed record BeginTwoFactorSetupCommand : IRequest<Result<TwoFactorSetupResult>>;
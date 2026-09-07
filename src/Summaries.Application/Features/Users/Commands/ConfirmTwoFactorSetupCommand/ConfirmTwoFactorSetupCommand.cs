using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Users.Commands.EnableTwoFactor;

public sealed record ConfirmTwoFactorSetupCommand(string Code) : IRequest<Result>;
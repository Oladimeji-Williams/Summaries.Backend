using MediatR;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Users.Application.Commands.ConfirmTwoFactorSetupCommand;

public sealed record ConfirmTwoFactorSetupCommand(string Code) : IRequest<Result>;
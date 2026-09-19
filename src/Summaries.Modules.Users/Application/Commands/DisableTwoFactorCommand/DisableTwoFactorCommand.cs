using MediatR;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Users.Application.Commands.DisableTwoFactorCommand;

public sealed record DisableTwoFactorCommand(string CurrentPassword) : IRequest<Result>;
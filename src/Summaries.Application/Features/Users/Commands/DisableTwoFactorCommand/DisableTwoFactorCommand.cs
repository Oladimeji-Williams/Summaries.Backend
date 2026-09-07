using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Users.Commands.DisableTwoFactor;

public sealed record DisableTwoFactorCommand(string CurrentPassword) : IRequest<Result>;
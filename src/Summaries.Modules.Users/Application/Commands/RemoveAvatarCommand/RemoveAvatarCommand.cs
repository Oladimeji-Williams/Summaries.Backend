using MediatR;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Users.Application.Commands.RemoveAvatarCommand;

public sealed record RemoveAvatarCommand : IRequest<Result>;
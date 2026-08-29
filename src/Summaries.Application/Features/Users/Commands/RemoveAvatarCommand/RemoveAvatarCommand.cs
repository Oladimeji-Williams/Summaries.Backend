using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Users.Commands.RemoveAvatar;

public sealed record RemoveAvatarCommand : IRequest<Result>;
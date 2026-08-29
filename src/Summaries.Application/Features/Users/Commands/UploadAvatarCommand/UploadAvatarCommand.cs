using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Users.Commands.UploadAvatar;

public sealed record UploadAvatarCommand(
    Stream Content, string FileName, string ContentType, long Length)
    : IRequest<Result<string>>;
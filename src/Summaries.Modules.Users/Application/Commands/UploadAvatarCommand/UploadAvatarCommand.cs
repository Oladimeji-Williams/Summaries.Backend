using MediatR;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Users.Application.Commands.UploadAvatarCommand;

public sealed record UploadAvatarCommand(
    Stream Content, string FileName, string ContentType, long Length)
    : IRequest<Result<string>>;
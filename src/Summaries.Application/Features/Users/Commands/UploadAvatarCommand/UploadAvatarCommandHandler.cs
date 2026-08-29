using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Abstractions.Storage;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Users.Shared.Errors;

namespace Summaries.Application.Features.Users.Commands.UploadAvatar;

public sealed class UploadAvatarCommandHandler(
    ICurrentUser currentUser,
    IIdentityService identityService,
    IFileStorageService fileStorage)
    : IRequestHandler<UploadAvatarCommand, Result<string>>
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/webp",
    };
    private const long MaxSizeBytes = 2 * 1024 * 1024;

    public async Task<Result<string>> Handle(
        UploadAvatarCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Result<string>.Failure(UserErrors.NotAuthenticated());
        }

        if (!AllowedContentTypes.Contains(request.ContentType))
        {
            return Result<string>.Failure(
                UserErrors.InvalidFile("Only JPEG, PNG, or WebP images are allowed."));
        }

        if (request.Length > MaxSizeBytes)
        {
            return Result<string>.Failure(
                UserErrors.InvalidFile("Image must be 2MB or smaller."));
        }

        var extension = request.ContentType switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => ".jpg",
        };
        var fileName = $"{currentUser.UserId.Value:N}{extension}";

        var avatarUrl = await fileStorage.SaveAsync(
            request.Content, fileName, request.ContentType, cancellationToken);

        var result = await identityService.UpdateAvatarAsync(
            currentUser.UserId.Value, avatarUrl, cancellationToken);

        if (result.IsFailure)
        {
            return Result<string>.Failure(result.Errors);
        }

        return Result<string>.Success(avatarUrl);
    }
}
using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Abstractions.Storage;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Users.Shared.Errors;

namespace Summaries.Application.Features.Users.Commands.UploadAvatar;

public sealed class UploadAvatarCommandHandler(
    ICurrentUser currentUser,
    IIdentityService identityService,
    IFileStorageService fileStorage,
    IImageValidator imageValidator)
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

        var isValidImage = await imageValidator.IsValidImageAsync(request.Content, cancellationToken);
        if (!isValidImage)
        {
            return Result<string>.Failure(
                UserErrors.InvalidFile("The uploaded file is not a valid image."));
        }

        var profile = await identityService.GetProfileAsync(currentUser.UserId.Value, cancellationToken);
        if (profile is null)
        {
            return Result<string>.Failure(UserErrors.NotFound(currentUser.UserId.Value));
        }

        var extension = request.ContentType switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => ".jpg",
        };
        var safeEmail = SanitizeForPublicId(profile.Email);
        var fileName = $"{safeEmail}{extension}";

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

    private static string SanitizeForPublicId(string email)
    {
        // Cloudinary public IDs allow letters, numbers, underscores, hyphens.
        // "@" and "." need replacing to stay filesystem/URL-safe.
        return email.Trim().ToLowerInvariant().Replace("@", "_at_").Replace(".", "_");
    }
}
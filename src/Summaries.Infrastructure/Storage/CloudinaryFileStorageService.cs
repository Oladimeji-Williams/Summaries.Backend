using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Summaries.Application.Abstractions.Storage;

namespace Summaries.Infrastructure.Storage;

internal sealed class CloudinaryFileStorageService(Cloudinary cloudinary) : IFileStorageService
{
    private const string Folder = "summaries/avatars";

    public async Task<string> SaveAsync(
        Stream content, string fileName, string contentType, CancellationToken cancellationToken)
    {
        // fileName arrives as "<sanitized-email>.<ext>" — see UploadAvatarCommandHandler.
        // Deleting first ensures a user never accumulates multiple images under
        // slightly different public IDs (e.g. if the extension changes between uploads).
        await DeleteExistingAsync(fileName);

        var publicId = Path.GetFileNameWithoutExtension(fileName);

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, content),
            Folder = Folder,
            PublicId = publicId,
            Overwrite = true,
            UniqueFilename = false,
            UseFilename = false,
        };

        var result = await cloudinary.UploadAsync(uploadParams, cancellationToken);

        if (result.Error is not null)
        {
            throw new InvalidOperationException($"Cloudinary upload failed: {result.Error.Message}");
        }

        var url = result.SecureUrl?.ToString() ?? result.Url?.ToString();
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new InvalidOperationException("Cloudinary did not return a URL for the uploaded image.");
        }

        return url;
    }

    public async Task DeleteAsync(string secureUrl, CancellationToken cancellationToken)
    {
        var publicId = ExtractPublicId(secureUrl);
        if (publicId is null)
        {
            return;
        }

        var deletionParams = new DeletionParams(publicId) { ResourceType = ResourceType.Image };
        await cloudinary.DestroyAsync(deletionParams);
    }

    private async Task DeleteExistingAsync(string fileName)
    {
        var publicId = $"{Folder}/{Path.GetFileNameWithoutExtension(fileName)}";
        var deletionParams = new DeletionParams(publicId) { ResourceType = ResourceType.Image };
        // Best-effort — if nothing exists at this public ID yet, Cloudinary
        // returns "not found" rather than throwing, so this is safe on first upload.
        await cloudinary.DestroyAsync(deletionParams);
    }

    private static string? ExtractPublicId(string secureUrl)
    {
        var uploadIndex = secureUrl.IndexOf("/upload/", StringComparison.OrdinalIgnoreCase);
        if (uploadIndex < 0)
        {
            return null;
        }

        var afterUpload = secureUrl[(uploadIndex + "/upload/".Length)..];

        if (afterUpload.StartsWith('v'))
        {
            var slashIndex = afterUpload.IndexOf('/');
            if (slashIndex > 0 && afterUpload[1..slashIndex].All(char.IsDigit))
            {
                afterUpload = afterUpload[(slashIndex + 1)..];
            }
        }

        var extensionIndex = afterUpload.LastIndexOf('.');
        return extensionIndex > 0 ? afterUpload[..extensionIndex] : afterUpload;
    }
}
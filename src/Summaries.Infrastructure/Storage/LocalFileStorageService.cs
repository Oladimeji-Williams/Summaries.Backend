using Microsoft.AspNetCore.Hosting;
using Summaries.Application.Abstractions.Storage;

namespace Summaries.Infrastructure.Storage;

internal sealed class LocalFileStorageService(IWebHostEnvironment environment) : IFileStorageService
{
    private const string AvatarsFolder = "uploads/avatars";

    public async Task<string> SaveAsync(
        Stream content, string fileName, string contentType, CancellationToken cancellationToken)
    {
        var webRoot = environment.WebRootPath
            ?? Path.Combine(environment.ContentRootPath, "wwwroot");

        var folder = Path.Combine(webRoot, AvatarsFolder);
        Directory.CreateDirectory(folder);

        var filePath = Path.Combine(folder, fileName);
        await using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await content.CopyToAsync(fileStream, cancellationToken);
        }

        return $"/{AvatarsFolder}/{fileName}";
    }

    public Task DeleteAsync(string relativeUrl, CancellationToken cancellationToken)
    {
        var webRoot = environment.WebRootPath
            ?? Path.Combine(environment.ContentRootPath, "wwwroot");
        var fileName = Path.GetFileName(relativeUrl);
        var filePath = Path.Combine(webRoot, AvatarsFolder, fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        return Task.CompletedTask;
    }
}
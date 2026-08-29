namespace Summaries.Application.Abstractions.Storage;

public interface IFileStorageService
{
    Task<string> SaveAsync(
        Stream content, string fileName, string contentType, CancellationToken cancellationToken);

    Task DeleteAsync(string relativeUrl, CancellationToken cancellationToken);
}
namespace Summaries.Application.Abstractions.Storage;

public interface IFileStorageService
{
    Task<string> SaveAsync(
        Stream content, string fileName, string contentType, CancellationToken cancellationToken);

    Task DeleteAsync(string relativeUrl, CancellationToken cancellationToken);

    Task<string> SaveDocumentAsync(
        Stream content, string fileName, string contentType, CancellationToken cancellationToken);

    Task DeleteDocumentAsync(string relativeUrl, CancellationToken cancellationToken);
}
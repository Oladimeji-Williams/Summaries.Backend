namespace Summaries.Application.Abstractions.Storage;

public interface IImageValidator
{
    Task<bool> IsValidImageAsync(Stream content, CancellationToken cancellationToken);
}
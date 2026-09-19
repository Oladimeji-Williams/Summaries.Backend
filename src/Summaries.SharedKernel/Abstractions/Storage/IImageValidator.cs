namespace Summaries.SharedKernel.Abstractions.Storage;

public interface IImageValidator
{
    Task<bool> IsValidImageAsync(Stream content, CancellationToken cancellationToken);
}
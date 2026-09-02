using Summaries.Application.Abstractions.Storage;

namespace Summaries.Infrastructure.Storage;

internal sealed class ImageValidator : IImageValidator
{
    public Task<bool> IsValidImageAsync(Stream content, CancellationToken cancellationToken)
    {
        return ImageSignatureValidator.IsValidImageAsync(content, cancellationToken);
    }
}
using Summaries.SharedKernel.Abstractions.Storage;

namespace Summaries.Shared.Infrastructure.Storage;

internal sealed class ImageValidator : IImageValidator
{
    public Task<bool> IsValidImageAsync(Stream content, CancellationToken cancellationToken)
    {
        return ImageSignatureValidator.IsValidImageAsync(content, cancellationToken);
    }
}
namespace Summaries.Shared.Infrastructure.Urls;

public interface IUrlBuilder
{
    string? ToAbsoluteUrl(string? relativeUrl);
}
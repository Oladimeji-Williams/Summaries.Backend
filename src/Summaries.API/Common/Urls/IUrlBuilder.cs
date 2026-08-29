namespace Summaries.API.Common.Urls;

public interface IUrlBuilder
{
    string? ToAbsoluteUrl(string? relativeUrl);
}
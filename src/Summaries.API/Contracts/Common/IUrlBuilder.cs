namespace Summaries.API.Contracts.Common;

public interface IUrlBuilder
{
    string? ToAbsoluteUrl(string? relativeUrl);
}
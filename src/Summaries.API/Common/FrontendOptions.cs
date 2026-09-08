namespace Summaries.API.Common;
public sealed class FrontendOptions
{
    public const string SectionName = "Frontend";
    public string BaseUrl { get; init; } = null!; // e.g. "http://localhost:4200"
}
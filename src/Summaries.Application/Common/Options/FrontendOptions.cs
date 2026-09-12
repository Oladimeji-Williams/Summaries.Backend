namespace Summaries.Application.Common.Options;

public sealed class FrontendOptions
{
    public const string SectionName = "Frontend";
    public string BaseUrl { get; init; } = null!;
}
namespace Summaries.SharedKernel.Common.Options;

public sealed class FrontendOptions
{
    public const string SectionName = "Frontend";
    public string BaseUrl { get; init; } = null!;
}
namespace Summaries.Infrastructure.Email;

public sealed class BrandingOptions
{
    public const string SectionName = "Branding";

    public string LogoUrl { get; init; } = string.Empty;
}
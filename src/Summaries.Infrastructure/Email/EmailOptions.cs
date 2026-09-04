namespace Summaries.Infrastructure.Email;

public sealed class EmailOptions
{
    public const string SectionName = "Email";

    public string ApiKey { get; init; } = null!;
    public string FromAddress { get; init; } = null!;
    public string FromName { get; init; } = null!;
}
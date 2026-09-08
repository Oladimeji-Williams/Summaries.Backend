namespace Summaries.Infrastructure.Authentication;

public sealed class ExternalAuthOptions
{
    public const string SectionName = "Authentication";
    public ProviderCredentials Google { get; init; } = new();
    public ProviderCredentials Microsoft { get; init; } = new();
    public ProviderCredentials Facebook { get; init; } = new();
    public ProviderCredentials LinkedIn { get; init; } = new();
    public ProviderCredentials Twitter { get; init; } = new();
}

public sealed class ProviderCredentials
{
    public string ClientId { get; init; } = "";
    public string ClientSecret { get; init; } = "";
}
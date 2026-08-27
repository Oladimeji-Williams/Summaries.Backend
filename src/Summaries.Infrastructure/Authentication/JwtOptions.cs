namespace Summaries.Infrastructure.Authentication;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = null!;

    public string Audience { get; init; } = null!;

    public string SecretKey { get; init; } = null!;

    public int AccessTokenExpirationMinutes { get; init; }

    public int RefreshTokenExpirationDays { get; init; }
}
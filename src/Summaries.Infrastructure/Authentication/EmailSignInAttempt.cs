namespace Summaries.Infrastructure.Authentication;

public sealed class EmailSignInAttempt
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string CodeHash { get; set; } = null!;
    public string TokenHash { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? ConsumedAtUtc { get; set; }
    public int FailedAttempts { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
}
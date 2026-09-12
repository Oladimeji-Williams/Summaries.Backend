namespace Summaries.Infrastructure.Payments;

public sealed class PaystackOptions
{
    public const string SectionName = "Paystack";
    public string SecretKey { get; init; } = null!;
}
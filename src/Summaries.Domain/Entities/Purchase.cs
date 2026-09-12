using Summaries.Domain.Common;
using Summaries.Domain.Enums;

namespace Summaries.Domain.Entities;

public sealed class Purchase : Entity
{
    public int BookId { get; private set; }
    public Guid UserId { get; private set; }
    public long AmountKobo { get; private set; }
    public string PaystackReference { get; private set; } = null!;
    public PurchaseStatus Status { get; private set; }
    public DateTimeOffset? CompletedAtUtc { get; private set; }

    private Purchase() { }

    public Purchase(int bookId, Guid userId, long amountKobo, string paystackReference)
    {
        BookId = bookId;
        UserId = userId;
        AmountKobo = amountKobo;
        PaystackReference = paystackReference;
        Status = PurchaseStatus.Pending;
    }

    public void MarkSuccessful(DateTimeOffset completedAtUtc)
    {
        if (Status == PurchaseStatus.Successful)
        {
            return; // idempotent — webhook and manual verify can both land here
        }
        Status = PurchaseStatus.Successful;
        CompletedAtUtc = completedAtUtc;
    }

    public void MarkFailed()
    {
        if (Status == PurchaseStatus.Successful)
        {
            return; // never downgrade a completed purchase
        }
        Status = PurchaseStatus.Failed;
    }
}
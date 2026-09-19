using Summaries.SharedKernel.Contracts.Payments;
using Summaries.Modules.Payments.Application.Abstractions;

namespace Summaries.Modules.Payments.Application.CrossModule;

/// <summary>
/// Adapts the Payments module's own repository to the narrow contract
/// SharedKernel.Contracts.Payments exposes to other modules (Books, to
/// gate downloads on a successful purchase).
/// </summary>
public sealed class PurchaseVerifierService(IPurchaseRepository purchaseRepository) : IPurchaseVerifier
{
    public async Task<bool> HasSuccessfulPurchaseAsync(Guid userId, int bookId, CancellationToken cancellationToken = default)
    {
        var purchase = await purchaseRepository.GetSuccessfulForUserAndBookAsync(userId, bookId, cancellationToken);
        return purchase is not null;
    }

    public Task<IReadOnlyList<int>> GetPurchasedBookIdsAsync(Guid userId, CancellationToken cancellationToken = default) =>
        purchaseRepository.GetSuccessfulBookIdsForUserAsync(userId, cancellationToken);
}

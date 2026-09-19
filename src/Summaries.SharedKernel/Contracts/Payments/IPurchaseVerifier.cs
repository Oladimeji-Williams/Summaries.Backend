namespace Summaries.SharedKernel.Contracts.Payments;

/// <summary>
/// The one thing another module (Books) needs to know about purchases:
/// whether a given user has successfully bought a given book. Implemented
/// by the Payments module. Deliberately narrower than the Payments module's
/// own IPurchaseRepository, which stays private to Payments.
/// </summary>
public interface IPurchaseVerifier
{
    Task<bool> HasSuccessfulPurchaseAsync(Guid userId, int bookId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<int>> GetPurchasedBookIdsAsync(Guid userId, CancellationToken cancellationToken = default);
}

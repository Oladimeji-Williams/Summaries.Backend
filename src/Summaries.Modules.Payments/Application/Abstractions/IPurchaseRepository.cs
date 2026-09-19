
using Summaries.Modules.Payments.Domain.Entities;
namespace Summaries.Modules.Payments.Application.Abstractions;

public interface IPurchaseRepository
{
    Task AddAsync(Purchase purchase, CancellationToken cancellationToken);
    Task UpdateAsync(Purchase purchase, CancellationToken cancellationToken);
    Task<Purchase?> GetByReferenceAsync(string reference, CancellationToken cancellationToken);
    Task<Purchase?> GetSuccessfulForUserAndBookAsync(Guid userId, int bookId, CancellationToken cancellationToken);
    Task<Purchase?> GetPendingForUserAndBookAsync(Guid userId, int bookId, CancellationToken cancellationToken);
    Task<IReadOnlyList<int>> GetSuccessfulBookIdsForUserAsync(Guid userId, CancellationToken cancellationToken);
}
using Microsoft.EntityFrameworkCore;
using Summaries.Modules.Payments.Application.Abstractions;
using Summaries.Modules.Payments.Persistence;

using Summaries.Modules.Payments.Domain.Entities;
using Summaries.Modules.Payments.Domain.Enums;
namespace Summaries.Modules.Payments.Persistence.Repositories;

internal sealed class PurchaseRepository(PaymentsDbContext dbContext) : IPurchaseRepository
{
    public async Task AddAsync(Purchase purchase, CancellationToken cancellationToken)
    {
        await dbContext.Purchases.AddAsync(purchase, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Purchase purchase, CancellationToken cancellationToken)
    {
        dbContext.Purchases.Update(purchase);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Purchase?> GetByReferenceAsync(string reference, CancellationToken cancellationToken)
    {
        return dbContext.Purchases.FirstOrDefaultAsync(p => p.PaystackReference == reference, cancellationToken);
    }

    public Task<Purchase?> GetSuccessfulForUserAndBookAsync(Guid userId, int bookId, CancellationToken cancellationToken)
    {
        return dbContext.Purchases.FirstOrDefaultAsync(
            p => p.UserId == userId && p.BookId == bookId && p.Status == PurchaseStatus.Successful,
            cancellationToken);
    }

    public Task<Purchase?> GetPendingForUserAndBookAsync(Guid userId, int bookId, CancellationToken cancellationToken)
    {
        return dbContext.Purchases.FirstOrDefaultAsync(
            p => p.UserId == userId && p.BookId == bookId && p.Status == PurchaseStatus.Pending,
            cancellationToken);
    }

    public async Task<IReadOnlyList<int>> GetSuccessfulBookIdsForUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.Purchases
            .Where(p => p.UserId == userId && p.Status == PurchaseStatus.Successful)
            .Select(p => p.BookId)
            .ToListAsync(cancellationToken);
    }
}
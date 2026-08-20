using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Summaries.Domain.Common;

namespace Summaries.Persistence.Data.Interceptors;

public sealed class AuditingInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateAuditFields(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditFields(eventData.Context);

        return base.SavingChangesAsync(
            eventData,
            result,
            cancellationToken);
    }

    private static void UpdateAuditFields(
        DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var utcNow = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<Entity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.GetType();

                    entry.Property(
                        nameof(Entity.CreatedAt))
                        .CurrentValue = utcNow;

                    entry.Property(
                        nameof(Entity.ModifiedAt))
                        .CurrentValue = utcNow;

                    break;

                case EntityState.Modified:
                    entry.Property(
                        nameof(Entity.ModifiedAt))
                        .CurrentValue = utcNow;

                    entry.Property(
                        nameof(Entity.CreatedAt))
                        .IsModified = false;

                    break;
            }
        }
    }
}
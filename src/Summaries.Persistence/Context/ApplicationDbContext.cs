using Microsoft.EntityFrameworkCore;
using Summaries.Domain.Common;
using Summaries.Domain.Entities;

namespace Summaries.Persistence.Context;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<Book> Books => Set<Book>();
    public DbSet<BookReadingRecord> BookReadingRecords => Set<BookReadingRecord>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model
                     .GetEntityTypes()
                     .Where(type =>
                         typeof(Entity).IsAssignableFrom(
                             type.ClrType)))
        {
            var method = typeof(ApplicationDbContext)
                .GetMethod(
                    nameof(ApplySoftDeleteFilter),
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Static)!
                .MakeGenericMethod(entityType.ClrType);

            method.Invoke(
                null,
                [modelBuilder]);
        }

        base.OnModelCreating(modelBuilder);
    }

    private static void ApplySoftDeleteFilter<TEntity>(
        ModelBuilder modelBuilder)
        where TEntity : Entity
    {
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(entity => !entity.IsDeleted);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Summaries.Domain.Entities;

namespace Summaries.Persistence.Data.Configurations;

public sealed class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.Property(p => p.PaystackReference).HasMaxLength(100).IsRequired();
        builder.HasIndex(p => p.PaystackReference).IsUnique();
        builder.HasIndex(p => new { p.UserId, p.BookId });
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Summaries.Domain.Entities;

namespace Summaries.Persistence.Data.Configurations;

public sealed class BookReadingRecordConfiguration : IEntityTypeConfiguration<BookReadingRecord>
{
    public void Configure(EntityTypeBuilder<BookReadingRecord> builder)
    {
        builder.ToTable("BookReadingRecords");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.BookId).IsRequired();
        builder.Property(r => r.UserId).IsRequired();
        builder.HasIndex(r => new { r.BookId, r.UserId }).IsUnique();
        builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(r => r.Rating).HasPrecision(3, 2);
        builder.Property(r => r.DateStarted);
        builder.Property(r => r.DateRead);
        builder.Property(r => r.CreatedAt).IsRequired();
        builder.Property(r => r.ModifiedAt);
        builder.Property(r => r.DeletedAt);
        builder.Property(r => r.IsDeleted).IsRequired();
        builder.HasOne<Book>().WithMany().HasForeignKey(r => r.BookId).OnDelete(DeleteBehavior.Cascade);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Summaries.Domain.Entities;

namespace Summaries.Persistence.Data.Configurations;

public sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");

        builder.HasKey(book => book.Id);

        builder.Property(book => book.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(book => book.Author)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(book => book.Description)
            .IsRequired()
            .HasMaxLength(5000);

        builder.Property(book => book.Rating)
            .HasPrecision(3, 2);

        builder.Property(book => book.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(book => book.DateStarted);

        builder.Property(book => book.DateRead);

        builder.Property(book => book.CreatedAt)
            .IsRequired();

        builder.Property(book => book.ModifiedAt);

        builder.Property(book => book.DeletedAt);

        builder.Property(book => book.IsDeleted)
            .IsRequired();
    }
}
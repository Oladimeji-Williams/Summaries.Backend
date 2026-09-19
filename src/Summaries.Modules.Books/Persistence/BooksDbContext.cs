using System.Reflection;

using Microsoft.EntityFrameworkCore;

using Summaries.Modules.Books.Domain.Entities;

namespace Summaries.Modules.Books.Persistence;

public sealed class BooksDbContext(DbContextOptions<BooksDbContext> options) : DbContext(options)
{
    public DbSet<Book> Books => Set<Book>();

    public DbSet<BookReadingRecord> BookReadingRecords => Set<BookReadingRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}

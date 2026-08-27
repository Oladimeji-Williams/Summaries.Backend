using Microsoft.EntityFrameworkCore;
using Summaries.Domain.Entities;
using Summaries.Persistence.Context;

namespace Summaries.DatabaseSeeder.SeedData;

public static class BookReadingRecordSeedData
{
    public static async Task SeedAsync(
        ApplicationDbContext dbContext,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (await dbContext.BookReadingRecords
            .IgnoreQueryFilters()
            .AnyAsync(r => r.UserId == userId, cancellationToken))
        {
            return;
        }

        var books = await dbContext.Books
            .OrderBy(b => b.Title)
            .ToListAsync(cancellationToken);

        if (books.Count < 3)
        {
            return;
        }

        // One book read, one in progress, rest untouched — gives you a
        // representative spread to manually verify against.
        var read = new BookReadingRecord(books[0].Id, userId);
        read.StartReading(DateTimeOffset.UtcNow.AddDays(-14));
        read.MarkAsRead(DateTimeOffset.UtcNow.AddDays(-2), 0.85m);

        var inProgress = new BookReadingRecord(books[1].Id, userId);
        inProgress.StartReading(DateTimeOffset.UtcNow.AddDays(-3));

        await dbContext.BookReadingRecords.AddRangeAsync(
            [read, inProgress], cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
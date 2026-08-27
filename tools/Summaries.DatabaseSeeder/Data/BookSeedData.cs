using Microsoft.EntityFrameworkCore;
using Summaries.Domain.Entities;
using Summaries.Persistence.Context;

namespace Summaries.DatabaseSeeder.SeedData;

public static class BookSeedData
{
    public static async Task SeedAsync(
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        if (await dbContext.Books
            .IgnoreQueryFilters()
            .AnyAsync(cancellationToken))
        {
            return;
        }
        var books = new[]
        {
            new Book(
                "Clean Architecture",
                "Robert C. Martin",
                "A practical guide to designing maintainable software systems using clean architecture principles."
            ),
            new Book(
                "The Pragmatic Programmer",
                "Andrew Hunt & David Thomas",
                "A collection of practical software engineering principles, techniques, and habits for becoming a better programmer."
            ),
            new Book(
                "Designing Data-Intensive Applications",
                "Martin Kleppmann",
                "An in-depth exploration of the principles and technologies behind reliable, scalable, and maintainable data-intensive applications."
            ),
            new Book(
                "Refactoring",
                "Martin Fowler",
                "A guide to improving the design of existing code while preserving its behavior."
            ),
            new Book(
                "Domain-Driven Design",
                "Eric Evans",
                "An approach to software development that focuses on modeling complex business domains and aligning software design with domain concepts."
            )
        };
        await dbContext.Books.AddRangeAsync(
            books,
            cancellationToken);
        await dbContext.SaveChangesAsync(
            cancellationToken);
    }
}
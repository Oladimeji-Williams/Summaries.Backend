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
                "A practical guide to designing maintainable software systems using clean architecture principles.",
                "9780134494166",
                "Prentice Hall",
                2017,
                "Software Engineering",
                432
            ),
            new Book(
                "The Pragmatic Programmer",
                "Andrew Hunt & David Thomas",
                "A collection of practical software engineering principles, techniques, and habits for becoming a better programmer.",
                "9780135957059",
                "Addison-Wesley",
                2019,
                "Software Engineering",
                352
            ),
            new Book(
                "Designing Data-Intensive Applications",
                "Martin Kleppmann",
                "An in-depth exploration of the principles and technologies behind reliable, scalable, and maintainable data-intensive applications.",
                "9781449373320",
                "O'Reilly Media",
                2017,
                "Software Engineering",
                616
            ),
            new Book(
                "Refactoring",
                "Martin Fowler",
                "A guide to improving the design of existing code while preserving its behavior.",
                "9780134757599",
                "Addison-Wesley",
                2018,
                "Software Engineering",
                448
            ),
            new Book(
                "Domain-Driven Design",
                "Eric Evans",
                "An approach to software development that focuses on modeling complex business domains and aligning software design with domain concepts.",
                "9780321125217",
                "Addison-Wesley",
                2003,
                "Software Engineering",
                560
            )
        };
        await dbContext.Books.AddRangeAsync(
            books,
            cancellationToken);
        await dbContext.SaveChangesAsync(
            cancellationToken);
    }
}
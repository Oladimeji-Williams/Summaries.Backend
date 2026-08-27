using Microsoft.EntityFrameworkCore;
using Summaries.Application.Abstractions.Persistence;
using Summaries.Domain.Entities;
using Summaries.Persistence.Context;

namespace Summaries.Persistence.Repositories;

public sealed class BookReadingRecordRepository(ApplicationDbContext dbContext)
    : IBookReadingRecordRepository
{
    public async Task<BookReadingRecord?> GetByUserAndBookAsync(
        Guid userId, int bookId, CancellationToken cancellationToken = default)
    {
        return await dbContext.BookReadingRecords
            .FirstOrDefaultAsync(r => r.UserId == userId && r.BookId == bookId, cancellationToken);
    }

    public async Task<IReadOnlyList<BookReadingRecord>> GetAllForUserAsync(
        Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.BookReadingRecords
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BookReadingRecord>> GetAllForBookAsync(
        int bookId, CancellationToken cancellationToken = default)
    {
        return await dbContext.BookReadingRecords
            .AsNoTracking()
            .Where(r => r.BookId == bookId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(BookReadingRecord record, CancellationToken cancellationToken = default)
    {
        await dbContext.BookReadingRecords.AddAsync(record, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(BookReadingRecord record, CancellationToken cancellationToken = default)
    {
        dbContext.BookReadingRecords.Update(record);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
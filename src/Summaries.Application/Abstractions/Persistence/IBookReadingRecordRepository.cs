using Summaries.Domain.Entities;

namespace Summaries.Application.Abstractions.Persistence;

public interface IBookReadingRecordRepository
{
    Task<BookReadingRecord?> GetByUserAndBookAsync(
        Guid userId, int bookId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookReadingRecord>> GetAllForUserAsync(
        Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookReadingRecord>> GetAllForBookAsync(
        int bookId, CancellationToken cancellationToken = default);
    Task AddAsync(BookReadingRecord record, CancellationToken cancellationToken = default);
    Task UpdateAsync(BookReadingRecord record, CancellationToken cancellationToken = default);
}
using Summaries.Domain.Entities;

namespace Summaries.Application.Abstractions.Persistence;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Book?> GetByTitleAsync(
        string title,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Book>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Book book,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Book book,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Book book,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        int id,
        CancellationToken cancellationToken = default);
}
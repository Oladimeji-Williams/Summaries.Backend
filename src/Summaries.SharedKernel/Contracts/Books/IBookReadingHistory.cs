namespace Summaries.SharedKernel.Contracts.Books;

/// <summary>
/// The read-only view of reading history that other modules may depend on.
/// Implemented by the Books module; consumed by Admin for cross-user/
/// cross-book reporting without a project reference to Books.
/// </summary>
public interface IBookReadingHistory
{
    Task<IReadOnlyList<ReadingRecordSummaryDto>> GetAllForBookAsync(int bookId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ReadingRecordSummaryDto>> GetAllForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}

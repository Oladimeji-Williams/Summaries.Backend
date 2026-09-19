namespace Summaries.SharedKernel.Contracts.Books;

/// <summary>
/// The read-only view of the book catalog that other modules may depend on.
/// Implemented by the Books module; consumed by Payments (price/PDF at
/// purchase time) and Admin (reporting) without either needing a project
/// reference to Books.
/// </summary>
public interface IBookCatalog
{
    Task<BookSummaryDto?> GetByIdAsync(int bookId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BookSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default);
}

using Summaries.SharedKernel.Contracts.Books;
using Summaries.Modules.Books.Application.Abstractions;

namespace Summaries.Modules.Books.Application.CrossModule;

/// <summary>
/// Adapts the Books module's own repositories to the narrow, read-only
/// contracts SharedKernel.Contracts.Books exposes to other modules
/// (Payments, Admin). Nothing outside Books ever sees a Book or
/// BookReadingRecord entity directly.
/// </summary>
public sealed class BookCatalogService(
    IBookRepository bookRepository,
    IBookReadingRecordRepository readingRecordRepository)
    : IBookCatalog, IBookReadingHistory
{
    public async Task<BookSummaryDto?> GetByIdAsync(
        int bookId,
        CancellationToken cancellationToken = default)
    {
        var book = await bookRepository.GetByIdAsync(
            bookId,
            cancellationToken);

        return book is null ? null : ToSummary(book);
    }

    public async Task<IReadOnlyList<BookSummaryDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var books = await bookRepository.GetAllAsync(cancellationToken);

        return books.Select(ToSummary).ToList();
    }

    public async Task<IReadOnlyList<ReadingRecordSummaryDto>> GetAllForBookAsync(
        int bookId,
        CancellationToken cancellationToken = default)
    {
        var records = await readingRecordRepository.GetAllForBookAsync(
            bookId,
            cancellationToken);

        return records.Select(ToSummary).ToList();
    }

    public async Task<IReadOnlyList<ReadingRecordSummaryDto>> GetAllForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var records = await readingRecordRepository.GetAllForUserAsync(
            userId,
            cancellationToken);

        return records.Select(ToSummary).ToList();
    }

    private static BookSummaryDto ToSummary(
        Domain.Entities.Book book) =>
        new(
            book.Id,
            book.Title,
            book.Author,
            book.PriceKobo,
            book.PdfUrl);

    private static ReadingRecordSummaryDto ToSummary(
        Domain.Entities.BookReadingRecord record) =>
        new(
            record.BookId,
            record.UserId,
            record.Status,
            record.Rating,
            record.DateStarted?.DateTime,
            record.DateRead?.DateTime);
}
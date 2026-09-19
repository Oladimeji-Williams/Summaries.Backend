using Summaries.Modules.Books.Application.DTOs;

using Summaries.Modules.Books.Domain.Entities;
namespace Summaries.Modules.Books.Application.Mappings;

public static class BookMappings
{
    public static BookDto ToDto(this Book book, BookReadingRecord? record, bool isPurchased)
    {
        return new BookDto(
            book.Id,
            book.Title,
            book.Author,
            book.Description,
            book.Isbn,
            book.Publisher,
            book.PublishedYear,
            book.Genre,
            book.PageCount,
            record is null
                ? null
                : new ReadingStatusDto(record.Status, record.Rating, record.DateStarted, record.DateRead),
            book.PriceKobo,
            !string.IsNullOrEmpty(book.PdfUrl),
            isPurchased);
    }
}
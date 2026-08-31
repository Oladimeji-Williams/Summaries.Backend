using Summaries.Application.Features.Books.Shared.DTOs;
using Summaries.Domain.Entities;

namespace Summaries.Application.Features.Books.Shared.Mappings;

public static class BookMappings
{
    public static BookDto ToDto(this Book book, BookReadingRecord? record)
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
                : new ReadingStatusDto(record.Status, record.Rating, record.DateStarted, record.DateRead));
    }
}
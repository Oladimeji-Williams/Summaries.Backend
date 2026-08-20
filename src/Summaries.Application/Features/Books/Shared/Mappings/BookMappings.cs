using Summaries.Application.Features.Books.Shared.DTOs;
using Summaries.Domain.Entities;

namespace Summaries.Application.Features.Books.Shared.Mappings;

public static class BookMappings
{
    public static BookDto ToDto(this Book book)
    {
        return new BookDto(
            book.Id,
            book.Title,
            book.Author,
            book.Description,
            book.Rating,
            book.DateStarted,
            book.DateRead,
            book.Status
        );
    }

    public static IReadOnlyList<BookDto> ToDto(
        this IEnumerable<Book> books)
    {
        return books
            .Select(book => book.ToDto())
            .ToList();
    }
}
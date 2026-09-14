namespace Summaries.API.Contracts.Books;

public sealed record CreateBookRequest(
    string Title,
    string Author,
    string Description,
    string? Isbn,
    string? Publisher,
    int? PublishedYear,
    string? Genre,
    int? PageCount);

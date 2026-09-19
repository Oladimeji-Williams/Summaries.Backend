namespace Summaries.Modules.Books.Api.Contracts;

public sealed record UpdateBookRequest(
    string Title,
    string Author,
    string Description,
    string? Isbn,
    string? Publisher,
    int? PublishedYear,
    string? Genre,
    int? PageCount);
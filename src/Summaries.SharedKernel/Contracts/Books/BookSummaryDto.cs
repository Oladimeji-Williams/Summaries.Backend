namespace Summaries.SharedKernel.Contracts.Books;

/// <summary>
/// The read-only shape of a book other modules (Payments, Admin) are allowed
/// to see. Deliberately not the actual Book entity — that stays private to
/// the Books module so it alone controls how books are created and changed.
/// </summary>
public sealed record BookSummaryDto(
    int Id,
    string Title,
    string Author,
    long? PriceKobo,
    string? PdfUrl);

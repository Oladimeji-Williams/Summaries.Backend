namespace Summaries.SharedKernel.Contracts.Books;

/// <summary>
/// The read-only shape of a reading record other modules (Admin) are
/// allowed to see. Not the actual BookReadingRecord entity — that stays
/// private to the Books module.
/// </summary>
public sealed record ReadingRecordSummaryDto(
    int BookId,
    Guid UserId,
    BookStatus Status,
    decimal? Rating,
    DateTime? DateStarted,
    DateTime? DateRead);

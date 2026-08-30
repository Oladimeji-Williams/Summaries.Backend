using Summaries.Domain.Enums;

namespace Summaries.Application.Features.Admin.Shared.DTOs;

public sealed record BookReadingEntryDto(
    int BookId, string Title, string Author,
    BookStatus Status, decimal? Rating,
    DateTimeOffset? DateStarted, DateTimeOffset? DateRead)
{
    public double? ReadingDurationHours =>
        DateStarted.HasValue && DateRead.HasValue
            ? (DateRead.Value - DateStarted.Value).TotalHours
            : null;
}

public sealed record UserReadingHistoryDto(
    Guid UserId, string Email, string FirstName, string LastName,
    IReadOnlyList<BookReadingEntryDto> Books);

public sealed record ReaderEntryDto(
    Guid UserId, string Email, string FirstName, string LastName,
    BookStatus Status, decimal? Rating,
    DateTimeOffset? DateStarted, DateTimeOffset? DateRead)
{
    public double? ReadingDurationHours =>
        DateStarted.HasValue && DateRead.HasValue
            ? (DateRead.Value - DateStarted.Value).TotalHours
            : null;
}

public sealed record BookReadersDto(
    int BookId, string Title, string Author,
    IReadOnlyList<ReaderEntryDto> Readers);
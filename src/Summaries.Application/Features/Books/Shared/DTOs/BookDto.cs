using Summaries.Domain.Enums;

namespace Summaries.Application.Features.Books.Shared.DTOs;

public sealed record BookDto(
    int Id,
    string Title,
    string Author,
    string Description,
    string? Isbn,
    string? Publisher,
    int? PublishedYear,
    string? Genre,
    int? PageCount,
    ReadingStatusDto? MyReadingStatus
);

public sealed record ReadingStatusDto(
    BookStatus Status,
    decimal? Rating,
    DateTimeOffset? DateStarted,
    DateTimeOffset? DateRead);
using Summaries.Domain.Enums;

namespace Summaries.Application.Features.Books.Shared.DTOs;

public sealed record BookDto(
    int Id,
    string Title,
    string Author,
    string Description,
    ReadingStatusDto? MyReadingStatus);

public sealed record ReadingStatusDto(
    BookStatus Status,
    decimal? Rating,
    DateTimeOffset? DateStarted,
    DateTimeOffset? DateRead);
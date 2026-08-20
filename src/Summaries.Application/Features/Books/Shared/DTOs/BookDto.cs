using Summaries.Domain.Enums;

namespace Summaries.Application.Features.Books.Shared.DTOs;

public sealed record BookDto(
    int Id,
    string Title,
    string Author,
    string Description,
    decimal? Rating,
    DateTimeOffset? DateStarted,
    DateTimeOffset? DateRead,
    BookStatus Status);
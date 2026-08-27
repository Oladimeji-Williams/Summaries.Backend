using Summaries.Domain.Common;
using Summaries.Domain.Enums;

namespace Summaries.Domain.Entities;

public sealed class BookReadingRecord : Entity
{
    public int BookId { get; private set; }
    public Guid UserId { get; private set; }
    public BookStatus Status { get; private set; }
    public decimal? Rating { get; private set; }
    public DateTimeOffset? DateStarted { get; private set; }
    public DateTimeOffset? DateRead { get; private set; }

    private BookReadingRecord() { }

    public BookReadingRecord(int bookId, Guid userId)
    {
        BookId = bookId;
        UserId = userId;
        Status = BookStatus.NotStarted;
    }

    public void StartReading(DateTimeOffset startedAt)
    {
        if (Status != BookStatus.NotStarted)
        {
            return;
        }
        Status = BookStatus.InProgress;
        DateStarted = startedAt;
    }

    public void MarkAsRead(DateTimeOffset readAt, decimal? rating)
    {
        if (Status != BookStatus.InProgress)
        {
            throw new InvalidOperationException(
                "Only a book that is in progress can be marked as read.");
        }
        Status = BookStatus.Read;
        DateRead = readAt;
        SetRating(rating);
    }

    public void SetRating(decimal? rating)
    {
        if (rating is null)
        {
            Rating = null;
            return;
        }
        if (rating < 0m || rating > 5m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rating), "Rating must be between 0.00 and 5.00.");
        }
        if (decimal.Round(rating.Value, 2) != rating.Value)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rating), "Rating must have at most two decimal places.");
        }
        Rating = rating;
    }
}
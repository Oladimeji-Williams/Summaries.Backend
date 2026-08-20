using Summaries.Domain.Common;
using Summaries.Domain.Enums;

namespace Summaries.Domain.Entities;

public sealed class Book : Entity
{
    public string Title { get; private set; } = null!;

    public string Author { get; private set; } = null!;

    public string Description { get; private set; } = null!;

    public decimal? Rating { get; private set; }

    public DateTimeOffset? DateStarted { get; private set; }

    public DateTimeOffset? DateRead { get; private set; }

    public BookStatus Status { get; private set; }

    private Book()
    {
    }

    public Book(
        string title,
        string author,
        string description,
        decimal? rating)
    {
        Title = title;
        Author = author;
        Description = description;

        SetRating(rating);

        Status = BookStatus.NotStarted;
        DateStarted = null;
        DateRead = null;
    }

    public void Update(
        string title,
        string author,
        string description,
        decimal? rating)
    {
        if (Status == BookStatus.Read)
        {
            throw new InvalidOperationException(
                "A book that has been read cannot be edited.");
        }

        Title = title;
        Author = author;
        Description = description;

        SetRating(rating);
    }

    public void SetRating(decimal? rating)
    {
        if (rating is null)
        {
            Rating = null;
            return;
        }

        if (rating < 0m || rating > 1m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rating),
                "Rating must be between 0.00 and 1.00.");
        }

        if (decimal.Round(rating.Value, 2) != rating.Value)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rating),
                "Rating must have at most two decimal places.");
        }

        Rating = rating;
    }

    public void StartReading(
        DateTimeOffset startedAt)
    {
        if (Status != BookStatus.NotStarted)
        {
            return;
        }

        Status = BookStatus.InProgress;
        DateStarted = startedAt;
    }

    public void MarkAsRead(
        DateTimeOffset readAt)
    {
        if (Status != BookStatus.InProgress)
        {
            throw new InvalidOperationException(
                "Only a book that is in progress can be marked as read.");
        }

        Status = BookStatus.Read;
        DateRead = readAt;
    }
}
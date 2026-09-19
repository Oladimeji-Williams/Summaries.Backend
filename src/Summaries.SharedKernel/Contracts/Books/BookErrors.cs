using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.SharedKernel.Contracts.Books;

public static class BookErrors
{
    public static Error NotFound(int id) => new(
        "Books.NotFound", $"Book with ID '{id}' was not found.", ErrorType.NotFound);

    public static Error AlreadyExists(string title) => new(
        "Books.AlreadyExists", $"A book with the title '{title}' already exists.", ErrorType.Conflict);

    public static Error NotStarted() => new(
        "Books.NotStarted", "This book has already been started.", ErrorType.Conflict);

    public static Error NotInProgress() => new(
        "Books.NotInProgress", "Only a book that is in progress can be marked as read.", ErrorType.Conflict);

    public static Error NotPurchased() => new(
        "Books.NotPurchased", "You need to purchase this book before downloading it.", ErrorType.Forbidden);

    public static Error NoPdfAvailable() => new(
        "Books.NoPdfAvailable", "This book does not have a PDF available yet.", ErrorType.NotFound);
}
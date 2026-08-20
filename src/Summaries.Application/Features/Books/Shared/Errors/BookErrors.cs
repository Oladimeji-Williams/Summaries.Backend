using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Books.Shared.Errors;

public static class BookErrors
{
    public static Error NotFound(int id)
    {
        return new Error(
            "Books.NotFound",
            $"Book with ID '{id}' was not found.",
            ErrorType.NotFound);
    }

    public static Error AlreadyExists(string title)
    {
        return new Error(
            "Books.AlreadyExists",
            $"A book with the title '{title}' already exists.",
            ErrorType.Conflict);
    }

    public static Error NotStarted()
    {
        return new Error(
            "Books.NotStarted",
            "This book has already been started.",
            ErrorType.Conflict);
    }

    public static Error NotInProgress()
    {
        return new Error(
            "Books.NotInProgress",
            "Only a book that is in progress can be marked as read.",
            ErrorType.Conflict);
    }

    public static Error AlreadyRead()
    {
        return new Error(
            "Books.AlreadyRead",
            "A book that has been read cannot be edited.",
            ErrorType.Conflict);
    }
}
namespace Summaries.SharedKernel.Contracts.Books;

/// <summary>
/// A reader's progress on a book. Lives here rather than in Modules.Books
/// because it's part of the vocabulary other modules (e.g. Admin) need to
/// display reading status without depending on the Books module itself.
/// </summary>
public enum BookStatus
{
    NotStarted = 0,
    InProgress = 1,
    Read = 2
}

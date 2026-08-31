using Summaries.Domain.Common;

namespace Summaries.Domain.Entities;

public sealed class Book : Entity
{
    public string Title { get; private set; } = null!;
    public string Author { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string? Isbn { get; private set; }
    public string? Publisher { get; private set; }
    public int? PublishedYear { get; private set; }
    public string? Genre { get; private set; }
    public int? PageCount { get; private set; }

    private Book() { }

    public Book(string title, string author, string description,
        string? isbn, string? publisher, int? publishedYear, string? genre, int? pageCount)
    {
        Title = title;
        Author = author;
        Description = description;
        Isbn = isbn;
        Publisher = publisher;
        PublishedYear = publishedYear;
        Genre = genre;
        PageCount = pageCount;
    }

    public void Update(string title, string author, string description,
        string? isbn, string? publisher, int? publishedYear, string? genre, int? pageCount)
    {
        Title = title;
        Author = author;
        Description = description;
        Isbn = isbn;
        Publisher = publisher;
        PublishedYear = publishedYear;
        Genre = genre;
        PageCount = pageCount;
    }
}
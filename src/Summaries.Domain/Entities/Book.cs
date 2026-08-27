using Summaries.Domain.Common;

namespace Summaries.Domain.Entities;

public sealed class Book : Entity
{
    public string Title { get; private set; } = null!;
    public string Author { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    private Book() { }

    public Book(string title, string author, string description)
    {
        Title = title;
        Author = author;
        Description = description;
    }

    public void Update(string title, string author, string description)
    {
        Title = title;
        Author = author;
        Description = description;
    }
}
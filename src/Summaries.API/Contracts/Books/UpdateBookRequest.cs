namespace Summaries.API.Contracts.Books;

public sealed record UpdateBookRequest(string Title, string Author, string Description);
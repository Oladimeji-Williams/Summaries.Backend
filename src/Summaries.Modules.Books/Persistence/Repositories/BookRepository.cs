using Microsoft.EntityFrameworkCore;
using Summaries.Modules.Books.Application.Abstractions;
using Summaries.Modules.Books.Persistence;

using Summaries.Modules.Books.Domain.Entities;
namespace Summaries.Modules.Books.Persistence.Repositories;

public sealed class BookRepository(
    BooksDbContext dbContext)
    : IBookRepository
{
    private readonly BooksDbContext _dbContext = dbContext;

    public async Task<Book?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Books
            .FirstOrDefaultAsync(
                book => book.Id == id,
                cancellationToken);
    }

    public async Task<Book?> GetByTitleAsync(
        string title,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Books
            .FirstOrDefaultAsync(
                book => book.Title == title,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Book>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Books
            .AsNoTracking()
            .OrderBy(book => book.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        Book book,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Books.AddAsync(
            book,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task UpdateAsync(
        Book book,
        CancellationToken cancellationToken = default)
    {
        _dbContext.Books.Update(book);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task DeleteAsync(
        Book book,
        CancellationToken cancellationToken = default)
    {
        book.Delete();

        _dbContext.Books.Update(book);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Books
            .AnyAsync(
                book => book.Id == id,
                cancellationToken);
    }
}
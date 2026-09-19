using MediatR;
using Summaries.Modules.Books.Application.Abstractions;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.Modules.Books.Application.DTOs;
using Summaries.SharedKernel.Contracts.Books;
using Summaries.Modules.Books.Application.Mappings;

using Summaries.Modules.Books.Domain.Entities;
namespace Summaries.Modules.Books.Application.Commands.CreateBook;

public sealed class CreateBookCommandHandler(IBookRepository bookRepository)
    : IRequestHandler<CreateBookCommand, Result<BookDto>>
{
    public async Task<Result<BookDto>> Handle(
        CreateBookCommand request, CancellationToken cancellationToken)
    {
        var existingBook = await bookRepository.GetByTitleAsync(request.Title, cancellationToken);
        if (existingBook is not null)
        {
            return Result<BookDto>.Failure(BookErrors.AlreadyExists(request.Title));
        }

        var book = new Book(
            request.Title,
            request.Author,
            request.Description,
            request.Isbn,
            request.Publisher,
            request.PublishedYear,
            request.Genre,
            request.PageCount);

        await bookRepository.AddAsync(book, cancellationToken);

        return Result<BookDto>.Success(book.ToDto(null, false));
    }
}
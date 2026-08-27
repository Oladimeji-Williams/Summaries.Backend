using MediatR;
using Summaries.Application.Abstractions.Persistence;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Books.Shared.DTOs;
using Summaries.Application.Features.Books.Shared.Errors;
using Summaries.Application.Features.Books.Shared.Mappings;
using Summaries.Domain.Entities;

namespace Summaries.Application.Features.Books.Commands.CreateBookCommand;

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

        var book = new Book(request.Title, request.Author, request.Description);
        await bookRepository.AddAsync(book, cancellationToken);

        return Result<BookDto>.Success(book.ToDto(null));
    }
}
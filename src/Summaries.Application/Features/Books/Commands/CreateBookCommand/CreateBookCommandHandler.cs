using MediatR;
using Summaries.Application.Abstractions.Persistence;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Books.Shared.DTOs;
using Summaries.Application.Features.Books.Shared.Errors;
using Summaries.Application.Features.Books.Shared.Mappings;
using Summaries.Domain.Entities;

namespace Summaries.Application.Features.Books.Commands.CreateBookCommand;

public sealed class CreateBookCommandHandler(
    IBookRepository bookRepository)
    : IRequestHandler<CreateBookCommand, Result<BookDto>>
{
    private readonly IBookRepository _bookRepository = bookRepository;

    public async Task<Result<BookDto>> Handle(
        CreateBookCommand request,
        CancellationToken cancellationToken)
    {
        var existingBook =
            await _bookRepository.GetByTitleAsync(
                request.Title,
                cancellationToken);

        if (existingBook is not null)
        {
            return Result<BookDto>.Failure(
                BookErrors.AlreadyExists(request.Title));
        }

        var book = new Book(
            request.Title,
            request.Author,
            request.Description,
            request.Rating);

        await _bookRepository.AddAsync(
            book,
            cancellationToken);

        var dto = book.ToDto();

        return Result<BookDto>.Success(dto);
    }
}
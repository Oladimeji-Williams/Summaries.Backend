using MediatR;
using Summaries.Application.Abstractions.Persistence;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Books.Shared.Errors;
using Summaries.Domain.Enums;

namespace Summaries.Application.Features.Books.Commands.UpdateBookCommand;

public sealed class UpdateBookCommandHandler(
    IBookRepository bookRepository)
    : IRequestHandler<UpdateBookCommand, Result>
{
    private readonly IBookRepository _bookRepository = bookRepository;

    public async Task<Result> Handle(
        UpdateBookCommand request,
        CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(
            request.Id,
            cancellationToken);
        if (book is null)
        {
            return Result.Failure(
                BookErrors.NotFound(request.Id));
        }
        var existingBook = await _bookRepository.GetByTitleAsync(
            request.Title,
            cancellationToken);
        if (existingBook is not null &&
            existingBook.Id != request.Id)
        {
            return Result.Failure(
                BookErrors.AlreadyExists(request.Title));
        }
        if (book.Status == BookStatus.Read)
        {
            return Result.Failure(
                BookErrors.AlreadyRead());
        }
        book.Update(
            request.Title,
            request.Author,
            request.Description,
            request.Rating);
        await _bookRepository.UpdateAsync(
            book,
            cancellationToken);
        return Result.Success();
    }
}
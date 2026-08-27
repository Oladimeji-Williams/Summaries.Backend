using MediatR;
using Summaries.Application.Abstractions.Persistence;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Books.Shared.Errors;

namespace Summaries.Application.Features.Books.Commands.DeleteBookCommand;

public sealed class DeleteBookCommandHandler(
    IBookRepository bookRepository)
    : IRequestHandler<DeleteBookCommand, Result>
{
    private readonly IBookRepository _bookRepository = bookRepository;

    public async Task<Result> Handle(
        DeleteBookCommand request,
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

        await _bookRepository.DeleteAsync(
            book,
            cancellationToken);

        return Result.Success();
    }
}
using MediatR;
using Summaries.Modules.Books.Application.Abstractions;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.SharedKernel.Contracts.Books;

namespace Summaries.Modules.Books.Application.Commands.DeleteBook;

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
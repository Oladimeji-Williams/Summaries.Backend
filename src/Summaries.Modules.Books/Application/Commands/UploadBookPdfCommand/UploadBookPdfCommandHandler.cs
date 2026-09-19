using MediatR;
using Summaries.Modules.Books.Application.Abstractions;
using Summaries.SharedKernel.Abstractions.Storage;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.SharedKernel.Contracts.Books;

namespace Summaries.Modules.Books.Application.Commands.UploadBookPdfCommand;

public sealed class UploadBookPdfCommandHandler(
    IBookRepository bookRepository, IFileStorageService fileStorage)
    : IRequestHandler<UploadBookPdfCommand, Result>
{
    public async Task<Result> Handle(UploadBookPdfCommand request, CancellationToken cancellationToken)
    {
        var book = await bookRepository.GetByIdAsync(request.BookId, cancellationToken);
        if (book is null)
        {
            return Result.Failure(BookErrors.NotFound(request.BookId));
        }

        var fileName = $"{request.BookId}.pdf";
        var url = await fileStorage.SaveDocumentAsync(
            request.Content, fileName, request.ContentType, cancellationToken);

        book.SetPdf(url);
        await bookRepository.UpdateAsync(book, cancellationToken);

        return Result.Success();
    }
}
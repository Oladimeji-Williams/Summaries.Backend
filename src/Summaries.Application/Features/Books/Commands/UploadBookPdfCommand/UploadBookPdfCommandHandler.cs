using MediatR;
using Summaries.Application.Abstractions.Persistence;
using Summaries.Application.Abstractions.Storage;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Books.Shared.Errors;

namespace Summaries.Application.Features.Books.Commands.UploadBookPdfCommand;

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
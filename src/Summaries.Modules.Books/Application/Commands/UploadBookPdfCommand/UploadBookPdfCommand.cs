using MediatR;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Books.Application.Commands.UploadBookPdfCommand;

public sealed record UploadBookPdfCommand(
    int BookId, Stream Content, string FileName, string ContentType) : IRequest<Result>;
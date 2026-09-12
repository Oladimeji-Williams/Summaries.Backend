using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Books.Commands.UploadBookPdfCommand;

public sealed record UploadBookPdfCommand(
    int BookId, Stream Content, string FileName, string ContentType) : IRequest<Result>;
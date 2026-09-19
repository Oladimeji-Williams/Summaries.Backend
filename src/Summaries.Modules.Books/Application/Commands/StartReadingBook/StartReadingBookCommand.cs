using MediatR;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Books.Application.Commands.StartReadingBook;

public sealed record StartReadingBookCommand(
    int BookId) : IRequest<Result>;
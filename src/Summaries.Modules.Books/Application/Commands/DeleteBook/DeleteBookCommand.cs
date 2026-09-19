using MediatR;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Books.Application.Commands.DeleteBook;

public sealed record DeleteBookCommand(
    int Id
) : IRequest<Result>;
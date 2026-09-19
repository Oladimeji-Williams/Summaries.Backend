using MediatR;
using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Books.Application.Queries.GetBookDownloadUrlQuery;

public sealed record GetBookDownloadUrlQuery(int BookId) : IRequest<Result<string>>;
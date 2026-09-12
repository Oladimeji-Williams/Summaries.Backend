using MediatR;
using Summaries.Application.Common.Primitives;

namespace Summaries.Application.Features.Books.Queries.GetBookDownloadUrlQuery;

public sealed record GetBookDownloadUrlQuery(int BookId) : IRequest<Result<string>>;
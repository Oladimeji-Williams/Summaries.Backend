using MediatR;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Admin.Shared.DTOs;

namespace Summaries.Application.Features.Admin.Queries.GetBookReaders;

public sealed record GetBookReadersQuery(int BookId) : IRequest<Result<BookReadersDto>>;
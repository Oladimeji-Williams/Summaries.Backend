using MediatR;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.Modules.Admin.Application.DTOs;

namespace Summaries.Modules.Admin.Application.Queries.GetBookReaders;

public sealed record GetBookReadersQuery(int BookId) : IRequest<Result<BookReadersDto>>;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Summaries.API.Contracts.Common;
using Summaries.API.Controllers.V1.Base;
using Summaries.Application.Features.Admin.Queries.GetAllUsers;
using Summaries.Application.Features.Admin.Queries.GetBookReaders;
using Summaries.Application.Features.Admin.Queries.GetUserReadingHistory;

namespace Summaries.API.Controllers.V1;

[ApiVersion(1.0)]
[Authorize(Roles = "Admin")]
public sealed class AdminController(ISender sender) : V1ControllerBase
{
    [HttpGet("users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllUsersQuery(), cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return Success(result.Value);
    }

    [HttpGet("users/{userId:guid}/reading-history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserReadingHistory(
        Guid userId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetUserReadingHistoryQuery(userId), cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return Success(result.Value);
    }

    [HttpGet("books/{bookId:int}/readers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBookReaders(
        int bookId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetBookReadersQuery(bookId), cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return Success(result.Value);
    }
}
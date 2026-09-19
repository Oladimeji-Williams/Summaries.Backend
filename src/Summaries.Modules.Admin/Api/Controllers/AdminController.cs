using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Summaries.Shared.Infrastructure.Contracts.Common;
using Summaries.Shared.Infrastructure.Api;
using Summaries.Modules.Admin.Application.Queries.GetAllUsers;
using Summaries.Modules.Admin.Application.Queries.GetBookReaders;
using Summaries.Modules.Admin.Application.Queries.GetUserReadingHistory;
using Microsoft.AspNetCore.Http;

namespace Summaries.Modules.Admin.Api.Controllers;

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
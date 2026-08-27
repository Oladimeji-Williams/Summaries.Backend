using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Summaries.API.Contracts.Common;
using Summaries.Application.Common.Primitives;

namespace Summaries.API.Controllers.V1.Base;

[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class V1ControllerBase
    : Controllers.Base.ApiControllerBase
{
    protected IActionResult Success<T>(T data)
    {
        return Ok(
            new ApiResponse<T>(
                Success: true,
                Data: data,
                Errors: null));
    }

    protected IActionResult Created<T>(
        string location,
        T data)
    {
        var response = new ApiResponse<T>(
            Success: true,
            Data: data,
            Errors: null);

        return new CreatedResult(
            location,
            response);
    }

    protected IActionResult Failure<T>(
        Result<T> result)
    {
        return Failure(result.Errors);
    }

    protected IActionResult Failure(
        Result result)
    {
        return Failure(result.Errors);
    }

    private IActionResult Failure(
        IReadOnlyList<Error> errors)
    {
        var response =
            new ApiResponse<object>(
                Success: false,
                Data: null,
                Errors: errors
                    .Select(error =>
                        new ApiError(
                            error.Code,
                            error.Message,
                            error.Type.ToString()))
                    .ToList());

        var errorType =
            errors
                .Select(error => error.Type)
                .FirstOrDefault();

        return errorType switch
        {
            ErrorType.Validation =>
                BadRequest(response),

            ErrorType.NotFound =>
                NotFound(response),

            ErrorType.Conflict =>
                Conflict(response),

            ErrorType.Unauthorized =>
                Unauthorized(response),

            ErrorType.Forbidden =>
                new ObjectResult(response)
                {
                    StatusCode =
                        StatusCodes.Status403Forbidden
                },

            _ =>
                new ObjectResult(response)
                {
                    StatusCode =
                        StatusCodes.Status500InternalServerError
                }
        };
    }
}
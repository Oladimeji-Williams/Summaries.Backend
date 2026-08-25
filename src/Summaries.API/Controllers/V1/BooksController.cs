using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Summaries.API.Contracts.Books;
using Summaries.API.Contracts.Common;
using Summaries.API.Controllers.V1.Base;
using Summaries.Application.Features.Books.Commands.CreateBookCommand;
using Summaries.Application.Features.Books.Commands.DeleteBookCommand;
using Summaries.Application.Features.Books.Commands.UpdateBookCommand;
using Summaries.Application.Features.Books.Commands.MarkBookAsReadCommand;
using Summaries.Application.Features.Books.Commands.StartReadingBookCommand;
using Summaries.Application.Features.Books.Queries.GetAllBooksQuery;
using Summaries.Application.Features.Books.Queries.GetBookByIdQuery;
using Summaries.Application.Features.Books.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Summaries.API.Controllers.V1;

[ApiVersion(1.0)]
[Authorize]
public sealed class BooksController(
    ISender sender)
    : V1ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost]
    [ProducesResponseType(
        typeof(ApiResponse<BookDto>),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateBook(
        [FromBody] CreateBookCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            command,
            cancellationToken);

        if (result.IsFailure)
        {
            return Failure(result);
        }

        var book = result.Value!;

        var location = Url.Action(
            nameof(GetBookById),
            values: new
            {
                id = book.Id,
                version = "1.0"
            });

        return Created(
            location ?? $"/api/v1/books/{book.Id}",
            new ApiResponse<BookDto>(
                Success: true,
                Data: book,
                Errors: null));
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(ApiResponse<IReadOnlyList<BookDto>>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllBooks(
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetAllBooksQuery(),
            cancellationToken);

        if (result.IsFailure)
        {
            return Failure(result);
        }

        return Success(result.Value);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(
        typeof(ApiResponse<BookDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBookById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetBookByIdQuery(id),
            cancellationToken);

        if (result.IsFailure)
        {
            return Failure(result);
        }

        return Success(result.Value);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateBook(
        int id,
        [FromBody] UpdateBookRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateBookCommand(
            id,
            request.Title,
            request.Author,
            request.Description,
            request.Rating);

        var result = await _sender.Send(
            command,
            cancellationToken);

        if (result.IsFailure)
        {
            return Failure(result);
        }

        return NoContent();
    }

    [HttpPost("{id:int}/start-reading")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status409Conflict)]
    public async Task<IActionResult> StartReadingBook(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new StartReadingBookCommand(id),
            cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return NoContent();
    }

    [HttpPost("{id:int}/mark-as-read")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status409Conflict)]
    public async Task<IActionResult> MarkBookAsRead(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new MarkBookAsReadCommand(id),
            cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBook(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new DeleteBookCommand(id),
            cancellationToken);

        if (result.IsFailure)
        {
            return Failure(result);
        }

        return NoContent();
    }
}
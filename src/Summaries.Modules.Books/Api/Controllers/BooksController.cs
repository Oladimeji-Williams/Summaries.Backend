using MediatR;
using Microsoft.AspNetCore.Mvc;
using Summaries.Modules.Books.Api.Contracts;
using Summaries.Shared.Infrastructure.Contracts.Common;
using Summaries.Shared.Infrastructure.Api;
using Summaries.Modules.Books.Application.Commands.CreateBook;
using Summaries.Modules.Books.Application.Commands.DeleteBook;
using Summaries.Modules.Books.Application.Commands.UpdateBook;
using Summaries.Modules.Books.Application.Commands.MarkBookAsRead;
using Summaries.Modules.Books.Application.Commands.StartReadingBook;
using Summaries.Modules.Books.Application.Queries.GetAllBooks;
using Summaries.Modules.Books.Application.Queries.GetBookById;
using Summaries.Modules.Books.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Summaries.Modules.Books.Application.Commands.UpdateBookPriceCommand;
using Summaries.Modules.Books.Application.Commands.UploadBookPdfCommand;
using Summaries.Modules.Books.Application.Queries.GetBookDownloadUrlQuery;
using Microsoft.AspNetCore.Http;

namespace Summaries.Modules.Books.Api.Controllers;

[Authorize]
public sealed class BooksController(
    ISender sender)
    : V1ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost]
    [Authorize(Roles = "Admin")]
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
        [FromBody] CreateBookRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateBookCommand(
            request.Title, request.Author, request.Description,
            request.Isbn, request.Publisher, request.PublishedYear, request.Genre, request.PageCount);

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
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateBook(
        int id, [FromBody] UpdateBookRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateBookCommand(
            id, request.Title, request.Author, request.Description,
            request.Isbn, request.Publisher, request.PublishedYear, request.Genre, request.PageCount);
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return NoContent();
    }

    [HttpPost("{id:int}/mark-as-read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> MarkBookAsRead(
        int id, [FromBody] MarkAsReadRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new MarkBookAsReadCommand(id, request.Rating), cancellationToken);
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


    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
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

    [HttpPut("{id:int}/price")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateBookPrice(
        int id, [FromBody] UpdateBookPriceRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateBookPriceCommand(id, request.PriceKobo), cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return NoContent();
    }

    [HttpPost("{id:int}/pdf")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [RequestSizeLimit(50_000_000)]
    public async Task<IActionResult> UploadBookPdf(int id, IFormFile file, CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();
        var result = await _sender.Send(
            new UploadBookPdfCommand(id, stream, file.FileName, file.ContentType), cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return NoContent();
    }

    [HttpGet("{id:int}/download")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DownloadBook(int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetBookDownloadUrlQuery(id), cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return Success(result.Value);
    }
}
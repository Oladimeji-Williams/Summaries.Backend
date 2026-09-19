using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Summaries.Shared.Infrastructure.Api;
using Summaries.Shared.Infrastructure.Contracts.Common;
using Summaries.SharedKernel.Abstractions.Payments;
using Summaries.Modules.Payments.Application.Commands.InitiatePurchaseCommand;
using Summaries.Modules.Payments.Application.Commands.VerifyPurchaseCommand;
using Microsoft.AspNetCore.Http;

namespace Summaries.Modules.Payments.Api.Controllers;

public sealed class PaymentsController(
    ISender sender,
    IPaystackService paystackService) : V1ControllerBase
{
    [HttpPost("purchase/{bookId:int}")]
    [Authorize]
    [ProducesResponseType(
        typeof(ApiResponse<InitiatePurchaseResultDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status409Conflict)]
    public async Task<IActionResult> InitiatePurchase(
        int bookId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new InitiatePurchaseCommand(bookId),
            cancellationToken);

        if (result.IsFailure)
        {
            return Failure(result);
        }

        return Success(result.Value);
    }

    [HttpPost("paystack/webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> PaystackWebhook(
        CancellationToken cancellationToken)
    {
        Request.EnableBuffering();

        using var reader = new StreamReader(
            Request.Body,
            leaveOpen: true);

        var rawBody = await reader.ReadToEndAsync(cancellationToken);
        Request.Body.Position = 0;

        var signature =
            Request.Headers["x-paystack-signature"].FirstOrDefault();

        if (!paystackService.VerifyWebhookSignature(
                rawBody,
                signature))
        {
            return Unauthorized();
        }

        using var doc =
            System.Text.Json.JsonDocument.Parse(rawBody);

        var eventType =
            doc.RootElement
                .GetProperty("event")
                .GetString();

        if (eventType == "charge.success")
        {
            var reference =
                doc.RootElement
                    .GetProperty("data")
                    .GetProperty("reference")
                    .GetString();

            if (!string.IsNullOrEmpty(reference))
            {
                await sender.Send(
                    new VerifyPurchaseCommand(reference),
                    cancellationToken);
            }
        }

        return Ok();
    }

    [HttpGet("verify/{reference}")]
    [Authorize]
    public async Task<IActionResult> VerifyPurchase(
        string reference,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new VerifyPurchaseCommand(reference),
            cancellationToken);

        if (result.IsFailure)
        {
            return Failure(result);
        }

        return Success(result.Value);
    }
}
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Summaries.API.Controllers.V1.Base;
using Summaries.Application.Abstractions.Payments;
using Summaries.Application.Features.Books.Commands.VerifyPurchaseCommand;

namespace Summaries.API.Controllers.V1;

public sealed class PaymentsController(ISender sender, IPaystackService paystackService) : V1ControllerBase
{
    [HttpPost("paystack/webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> PaystackWebhook(CancellationToken cancellationToken)
    {
        Request.EnableBuffering();
        using var reader = new StreamReader(Request.Body, leaveOpen: true);
        var rawBody = await reader.ReadToEndAsync(cancellationToken);
        Request.Body.Position = 0;

        var signature = Request.Headers["x-paystack-signature"].FirstOrDefault();
        if (!paystackService.VerifyWebhookSignature(rawBody, signature))
        {
            return Unauthorized();
        }

        using var doc = System.Text.Json.JsonDocument.Parse(rawBody);
        var eventType = doc.RootElement.GetProperty("event").GetString();

        if (eventType == "charge.success")
        {
            var reference = doc.RootElement.GetProperty("data").GetProperty("reference").GetString();
            if (!string.IsNullOrEmpty(reference))
            {
                await sender.Send(new VerifyPurchaseCommand(reference), cancellationToken);
            }
        }

        return Ok();
    }

    [HttpGet("verify/{reference}")]
    public async Task<IActionResult> VerifyPurchase(string reference, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new VerifyPurchaseCommand(reference), cancellationToken);
        if (result.IsFailure)
        {
            return Failure(result);
        }
        return Success(result.Value);
    }
}
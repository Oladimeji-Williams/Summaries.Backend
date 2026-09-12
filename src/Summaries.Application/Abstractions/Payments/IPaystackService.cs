namespace Summaries.Application.Abstractions.Payments;

public sealed record PaystackInitializeResult(string AuthorizationUrl, string AccessCode);

public sealed record PaystackVerifyResult(bool Success, long AmountKobo, string Reference);

public interface IPaystackService
{
    Task<PaystackInitializeResult> InitializeTransactionAsync(
        string email, long amountKobo, string reference, string callbackUrl, CancellationToken cancellationToken);

    Task<PaystackVerifyResult> VerifyTransactionAsync(string reference, CancellationToken cancellationToken);

    bool VerifyWebhookSignature(string rawBody, string? signatureHeader);
}
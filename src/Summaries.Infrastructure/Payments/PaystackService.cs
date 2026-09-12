using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Summaries.Application.Abstractions.Payments;

namespace Summaries.Infrastructure.Payments;

internal sealed class PaystackService(HttpClient httpClient, IOptions<PaystackOptions> options) : IPaystackService
{
    private readonly PaystackOptions _options = options.Value;

    public async Task<PaystackInitializeResult> InitializeTransactionAsync(
        string email, long amountKobo, string reference, string callbackUrl, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("transaction/initialize", new
        {
            email,
            amount = amountKobo,
            reference,
            callback_url = callbackUrl,
        }, cancellationToken);

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<PaystackInitializeResponse>(cancellationToken)
            ?? throw new InvalidOperationException("Paystack returned an empty initialize response.");

        return new PaystackInitializeResult(payload.Data.AuthorizationUrl, payload.Data.AccessCode);
    }

    public async Task<PaystackVerifyResult> VerifyTransactionAsync(string reference, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"transaction/verify/{Uri.EscapeDataString(reference)}", cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<PaystackVerifyResponse>(cancellationToken)
            ?? throw new InvalidOperationException("Paystack returned an empty verify response.");

        var success = payload.Data.Status == "success";
        return new PaystackVerifyResult(success, payload.Data.Amount, payload.Data.Reference);
    }

    public bool VerifyWebhookSignature(string rawBody, string? signatureHeader)
    {
        if (string.IsNullOrEmpty(signatureHeader))
        {
            return false;
        }

        var keyBytes = Encoding.UTF8.GetBytes(_options.SecretKey);
        var bodyBytes = Encoding.UTF8.GetBytes(rawBody);
        var computedHash = HMACSHA512.HashData(keyBytes, bodyBytes);
        var computedHex = Convert.ToHexStringLower(computedHash);

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computedHex), Encoding.UTF8.GetBytes(signatureHeader));
    }

    private sealed record PaystackInitializeResponse(PaystackInitializeData Data);
    private sealed record PaystackInitializeData(
        [property: JsonPropertyName("authorization_url")] string AuthorizationUrl,
        [property: JsonPropertyName("access_code")] string AccessCode);

    private sealed record PaystackVerifyResponse(PaystackVerifyData Data);
    private sealed record PaystackVerifyData(string Status, long Amount, string Reference);
}
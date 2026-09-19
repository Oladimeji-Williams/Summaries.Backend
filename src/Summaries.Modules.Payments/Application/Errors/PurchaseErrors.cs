using Summaries.SharedKernel.Common.Primitives;

namespace Summaries.Modules.Payments.Application.Errors;

/// <summary>
/// Payments-owned errors — not shared via SharedKernel.Contracts because
/// nothing outside this module needs to raise or inspect them.
/// </summary>
public static class PurchaseErrors
{
    public static Error NotForSale() => new(
        "Purchases.NotForSale", "This book does not have a price set.", ErrorType.Conflict);

    public static Error AlreadyPurchased() => new(
        "Purchases.AlreadyPurchased", "You already own this book.", ErrorType.Conflict);

    public static Error NotFound() => new(
        "Purchases.NotFound", "Purchase reference not found.", ErrorType.NotFound);
}

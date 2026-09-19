using MediatR;
using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.SharedKernel.Abstractions.Email;
using Summaries.SharedKernel.Abstractions.Payments;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.SharedKernel.Contracts.Books;
using Summaries.Modules.Payments.Application.Abstractions;
using Summaries.Modules.Payments.Application.Errors;
using Summaries.Modules.Payments.Domain.Enums;

namespace Summaries.Modules.Payments.Application.Commands.VerifyPurchaseCommand;

public sealed class VerifyPurchaseCommandHandler(
    IPurchaseRepository purchaseRepository,
    IPaystackService paystackService,
    IBookCatalog bookCatalog,
    IIdentityService identityService,
    IEmailSender emailSender,
    TimeProvider timeProvider)
    : IRequestHandler<VerifyPurchaseCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(VerifyPurchaseCommand request, CancellationToken cancellationToken)
    {
        var purchase = await purchaseRepository.GetByReferenceAsync(request.Reference, cancellationToken);
        if (purchase is null)
        {
            return Result<bool>.Failure(PurchaseErrors.NotFound());
        }

        var wasAlreadySuccessful = purchase.Status == PurchaseStatus.Successful;

        var verify = await paystackService.VerifyTransactionAsync(request.Reference, cancellationToken);

        if (verify.Success && verify.AmountKobo == purchase.AmountKobo)
        {
            purchase.MarkSuccessful(timeProvider.GetUtcNow());
        }
        else
        {
            purchase.MarkFailed();
        }

        await purchaseRepository.UpdateAsync(purchase, cancellationToken);

        var justCompleted = !wasAlreadySuccessful && purchase.Status == PurchaseStatus.Successful;
        if (justCompleted)
        {
            await TrySendPurchaseEmailAsync(purchase.UserId, purchase.BookId, cancellationToken);
        }

        return Result<bool>.Success(verify.Success);
    }

    private async Task TrySendPurchaseEmailAsync(Guid userId, int bookId, CancellationToken cancellationToken)
    {
        try
        {
            var profile = await identityService.GetProfileAsync(userId, cancellationToken);
            var book = await bookCatalog.GetByIdAsync(bookId, cancellationToken);

            if (profile is null || book is null || string.IsNullOrEmpty(book.PdfUrl))
            {
                return;
            }

            var sentAt = timeProvider.GetUtcNow().ToOffset(TimeSpan.FromHours(1));
            await emailSender.SendBookDeliveryAsync(profile.Email, book.Title, book.PdfUrl, sentAt, cancellationToken);
        }
        catch
        {
            // Best-effort — the purchase itself already succeeded and is recorded;
            // a failed notification email shouldn't fail the verify request.
        }
    }
}

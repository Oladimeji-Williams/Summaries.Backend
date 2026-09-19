using MediatR;
using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.SharedKernel.Abstractions.Email;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.SharedKernel.Contracts.Books;
using Summaries.SharedKernel.Contracts.Payments;
using Summaries.Modules.Books.Application.Abstractions;

namespace Summaries.Modules.Books.Application.Queries.GetBookDownloadUrlQuery;

public sealed class GetBookDownloadUrlQueryHandler(
    IBookRepository bookRepository,
    IPurchaseVerifier purchaseVerifier,
    ICurrentUser currentUser,
    IIdentityService identityService,
    IEmailSender emailSender,
    TimeProvider timeProvider)
    : IRequestHandler<GetBookDownloadUrlQuery, Result<string>>
{
    public async Task<Result<string>> Handle(GetBookDownloadUrlQuery request, CancellationToken cancellationToken)
    {
        var book = await bookRepository.GetByIdAsync(request.BookId, cancellationToken);
        if (book is null)
        {
            return Result<string>.Failure(BookErrors.NotFound(request.BookId));
        }

        if (string.IsNullOrEmpty(book.PdfUrl))
        {
            return Result<string>.Failure(BookErrors.NoPdfAvailable());
        }

        var isFree = book.PriceKobo is null;
        var isAdmin = currentUser.Roles.Contains("Admin");

        if (!isFree && !isAdmin)
        {
            if (currentUser.UserId is null)
            {
                return Result<string>.Failure(BookErrors.NotPurchased());
            }

            var hasPurchased = await purchaseVerifier.HasSuccessfulPurchaseAsync(
                currentUser.UserId.Value, request.BookId, cancellationToken);
            if (!hasPurchased)
            {
                return Result<string>.Failure(BookErrors.NotPurchased());
            }
        }

        if (currentUser.UserId is not null)
        {
            await TrySendDownloadEmailAsync(currentUser.UserId.Value, book.Title, book.PdfUrl, cancellationToken);
        }

        return Result<string>.Success(book.PdfUrl);
    }

    private async Task TrySendDownloadEmailAsync(
        Guid userId, string bookTitle, string pdfUrl, CancellationToken cancellationToken)
    {
        try
        {
            var profile = await identityService.GetProfileAsync(userId, cancellationToken);
            if (profile is null)
            {
                return;
            }

            var sentAt = timeProvider.GetUtcNow().ToOffset(TimeSpan.FromHours(1));
            await emailSender.SendBookDeliveryAsync(profile.Email, bookTitle, pdfUrl, sentAt, cancellationToken);
        }
        catch
        {
            // Best-effort — a failed email shouldn't block the actual download.
        }
    }
}

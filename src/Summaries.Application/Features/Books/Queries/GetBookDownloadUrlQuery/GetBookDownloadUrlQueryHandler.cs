using MediatR;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Abstractions.Email;
using Summaries.Application.Abstractions.Persistence;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Books.Shared.Errors;

namespace Summaries.Application.Features.Books.Queries.GetBookDownloadUrlQuery;

public sealed class GetBookDownloadUrlQueryHandler(
    IBookRepository bookRepository,
    IPurchaseRepository purchaseRepository,
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

            var purchase = await purchaseRepository.GetSuccessfulForUserAndBookAsync(
                currentUser.UserId.Value, request.BookId, cancellationToken);
            if (purchase is null)
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
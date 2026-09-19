using MediatR;
using Microsoft.Extensions.Options;
using Summaries.SharedKernel.Abstractions.Authentication;
using Summaries.SharedKernel.Abstractions.Payments;
using Summaries.SharedKernel.Common.Options;
using Summaries.SharedKernel.Common.Primitives;
using Summaries.SharedKernel.Contracts.Books;
using Summaries.Modules.Payments.Application.Abstractions;
using Summaries.Modules.Payments.Application.Errors;
using Summaries.Modules.Payments.Domain.Entities;

namespace Summaries.Modules.Payments.Application.Commands.InitiatePurchaseCommand;

public sealed class InitiatePurchaseCommandHandler(
    IBookCatalog bookCatalog,
    IPurchaseRepository purchaseRepository,
    IIdentityService identityService,
    IPaystackService paystackService,
    ICurrentUser currentUser,
    IOptions<FrontendOptions> frontendOptions)
    : IRequestHandler<InitiatePurchaseCommand, Result<InitiatePurchaseResultDto>>
{
    public async Task<Result<InitiatePurchaseResultDto>> Handle(
        InitiatePurchaseCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Result<InitiatePurchaseResultDto>.Failure(BookErrors.NotFound(request.BookId));
        }

        var book = await bookCatalog.GetByIdAsync(request.BookId, cancellationToken);
        if (book is null)
        {
            return Result<InitiatePurchaseResultDto>.Failure(BookErrors.NotFound(request.BookId));
        }

        if (book.PriceKobo is null)
        {
            return Result<InitiatePurchaseResultDto>.Failure(PurchaseErrors.NotForSale());
        }

        var existing = await purchaseRepository.GetSuccessfulForUserAndBookAsync(
            currentUser.UserId.Value, request.BookId, cancellationToken);
        if (existing is not null)
        {
            return Result<InitiatePurchaseResultDto>.Failure(PurchaseErrors.AlreadyPurchased());
        }

        var profile = await identityService.GetProfileAsync(currentUser.UserId.Value, cancellationToken);
        if (profile is null)
        {
            return Result<InitiatePurchaseResultDto>.Failure(BookErrors.NotFound(request.BookId));
        }

        var reference = $"SUM-{Guid.NewGuid():N}";
        var purchase = new Purchase(request.BookId, currentUser.UserId.Value, book.PriceKobo.Value, reference);
        await purchaseRepository.AddAsync(purchase, cancellationToken);

        var callbackUrl = $"{frontendOptions.Value.BaseUrl}/payments/callback";
        var initResult = await paystackService.InitializeTransactionAsync(
            profile.Email, book.PriceKobo.Value, reference, callbackUrl, cancellationToken);

        return Result<InitiatePurchaseResultDto>.Success(
            new InitiatePurchaseResultDto(initResult.AuthorizationUrl, reference));
    }
}

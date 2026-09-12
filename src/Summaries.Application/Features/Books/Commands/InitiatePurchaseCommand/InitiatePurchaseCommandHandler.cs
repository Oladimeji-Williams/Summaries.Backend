using MediatR;
using Microsoft.Extensions.Options;
using Summaries.Application.Abstractions.Authentication;
using Summaries.Application.Abstractions.Payments;
using Summaries.Application.Abstractions.Persistence;
using Summaries.Application.Common.Options;
using Summaries.Application.Common.Primitives;
using Summaries.Application.Features.Books.Shared.Errors;
using Summaries.Domain.Entities;

namespace Summaries.Application.Features.Books.Commands.InitiatePurchaseCommand;

public sealed class InitiatePurchaseCommandHandler(
    IBookRepository bookRepository,
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

        var book = await bookRepository.GetByIdAsync(request.BookId, cancellationToken);
        if (book is null)
        {
            return Result<InitiatePurchaseResultDto>.Failure(BookErrors.NotFound(request.BookId));
        }

        if (book.PriceKobo is null)
        {
            return Result<InitiatePurchaseResultDto>.Failure(BookErrors.NotForSale());
        }

        var existing = await purchaseRepository.GetSuccessfulForUserAndBookAsync(
            currentUser.UserId.Value, request.BookId, cancellationToken);
        if (existing is not null)
        {
            return Result<InitiatePurchaseResultDto>.Failure(BookErrors.AlreadyPurchased());
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
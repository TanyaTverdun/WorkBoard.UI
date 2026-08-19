using Refit;
using WorkBoard.Services.Abstraction.DTOs.Subscriptions;

namespace WorkBoard.Services.Servises.Subscriptions;

internal interface ISubscriptionApi
{
    [Post("/api/subscriptions/create-checkout-session")]
    Task<CheckoutSessionResponseDto> CreateCheckoutSessionAsync(
        CancellationToken cancellationToken = default);

    [Delete("/api/subscriptions/cancel")]
    Task CancelSubscriptionAsync(
        CancellationToken cancellationToken = default);
}

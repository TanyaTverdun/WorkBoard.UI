namespace WorkBoard.Services.Abstraction.Services;

public interface ISubscriptionService
{
    Task<string> CreateCheckoutSessionAsync(
        CancellationToken cancellationToken = default);

    Task CancelSubscriptionAsync(
        CancellationToken cancellationToken = default);
}

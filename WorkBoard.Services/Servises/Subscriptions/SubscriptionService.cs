using Microsoft.Extensions.Logging;
using Refit;
using WorkBoard.Services.Abstraction.Services;

namespace WorkBoard.Services.Servises.Subscriptions;

internal class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionApi _subscriptionApi;
    private readonly ILogger<SubscriptionService> _logger;

    public SubscriptionService(
        ISubscriptionApi subscriptionApi,
        ILogger<SubscriptionService> logger)
    {
        _subscriptionApi = subscriptionApi;
        _logger = logger;
    }

    public async Task<string> CreateCheckoutSessionAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _subscriptionApi.CreateCheckoutSessionAsync(
                cancellationToken);
            return response.Url;
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while creating checkout session." +
                " Status: {StatusCode}",
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while creating checkout session.");
            throw;
        }
    }

    public async Task CancelSubscriptionAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _subscriptionApi.CancelSubscriptionAsync(
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while cancelling subscription." +
                " Status: {StatusCode}",
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while cancelling subscription.");
            throw;
        }
    }
}

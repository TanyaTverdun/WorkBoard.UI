using Microsoft.AspNetCore.Components;
using MudBlazor;
using WorkBoard.Domain.Enums;
using WorkBoard.Services.Abstraction.Services;
using WorkBoard.Services.Abstraction.StateProviders;
using WorkBoard.UI.Components.Subscriptions;

namespace WorkBoard.UI.Pages;

public partial class SubscriptionsPage
{
    [Inject]
    private ISubscriptionService SubscriptionService { get; set; } = null!;

    [Inject]
    private ICurrentUserProvider CurrentUserProvider { get; set; } = null!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    private IDialogService DialogService { get; set; } = null!;

    private const string ProPlanPrice = "$15";

    private SubscriptionTier _currentPlan = SubscriptionTier.Free;
    private bool _isLoading = true;
    private bool _isProcessing;

    protected override async Task OnInitializedAsync()
    {
        await LoadSubscriptionAsync();
    }

    private async Task LoadSubscriptionAsync()
    {
        try
        {
            _isLoading = true;

            await CurrentUserProvider.LoadProfileAsync();

            if (CurrentUserProvider.Profile != null)
            {
                _currentPlan = CurrentUserProvider.Profile.SubscriptionTier;
            }
        }
        catch (Exception)
        {
            Snackbar.Add(
                "Failed to load subscription info",
                Severity.Error);
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task OnUpgradeClickAsync()
    {
        try
        {
            _isProcessing = true;

            var checkoutUrl = await SubscriptionService.CreateCheckoutSessionAsync();

            NavigationManager.NavigateTo(
                checkoutUrl, 
                forceLoad: true);
        }
        catch (Exception)
        {
            Snackbar.Add(
                "Failed to initiate checkout. Please try again later.", 
                Severity.Error);

            _isProcessing = false;
        }
    }

    private async Task OnDowngradeClickAsync()
    {
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true,
            CloseOnEscapeKey = true
        };

        var dialog = await DialogService.ShowAsync<DowngradeConfirmationDialog>(
            "Downgrade to Free",
            options);

        var result = await dialog.Result;

        if (result is null || result.Canceled)
        {
            return;
        }

        try
        {
            _isProcessing = true;

            await SubscriptionService.CancelSubscriptionAsync();

            _currentPlan = SubscriptionTier.Free;

            if (CurrentUserProvider.Profile != null)
            {
                CurrentUserProvider.Profile.SubscriptionTier = SubscriptionTier.Free;
                CurrentUserProvider.NotifyProfileChanged();
            }

            Snackbar.Add(
                "Your account has been downgraded to the Free plan",
                Severity.Warning);
        }
        catch (Exception)
        {
            Snackbar.Add(
                "Failed to downgrade the subscription",
                Severity.Error);
        }
        finally
        {
            _isProcessing = false;
        }
    }
}
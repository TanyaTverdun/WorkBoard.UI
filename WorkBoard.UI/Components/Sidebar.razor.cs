using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using WorkBoard.Domain.Constants;
using WorkBoard.Services.Abstraction.DTOs.Users;
using WorkBoard.Services.Abstraction.Hubs;
using WorkBoard.Services.Abstraction.StateProviders;

namespace WorkBoard.UI.Components;

public partial class Sidebar : IDisposable
{
    [Inject]
    private ICurrentUserProvider CurrentUserProvider { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IBoardHubService BoardHubService { get; set; } = default!;

    protected string SearchQuery { get; set; } = string.Empty;

    private bool _isProfileMenuOpen = false;
    protected override void OnInitialized()
    {
        CurrentUserProvider.OnProfileChanged += HandleProfileChanged;
        BoardHubService.OnUserAvatarUpdated += HandleUserAvatarUpdated;
    }

    private void ToggleProfileMenu()
    {
        _isProfileMenuOpen = !_isProfileMenuOpen;
    }

    private void GoToProfile()
    {
        _isProfileMenuOpen = false;

        NavigationManager.NavigateTo(AppRoutes.Profile);
    }

    private async Task SignOut()
    {
        _isProfileMenuOpen = false;

        NavigationManager.NavigateToLogout(AppRoutes.Logout);
    }

    private void HandleProfileChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    private void HandleUserAvatarUpdated(UserAvatarUpdatedDto data)
    {
        if (CurrentUserProvider.Profile != null &&
            CurrentUserProvider.Profile.Id == data.UserId)
        {
            CurrentUserProvider.Profile.AvatarColor = data.AvatarColor;
            CurrentUserProvider.Profile.AvatarUrl = data.AvatarUrl;
            InvokeAsync(StateHasChanged);
        }
    }

    public void Dispose()
    {
        CurrentUserProvider.OnProfileChanged -= HandleProfileChanged;
        BoardHubService.OnUserAvatarUpdated -= HandleUserAvatarUpdated;
    }
}

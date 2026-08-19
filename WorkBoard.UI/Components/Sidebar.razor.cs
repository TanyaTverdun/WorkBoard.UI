using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using WorkBoard.Domain.Constants;
using WorkBoard.Services.Abstraction.DTOs.Board;
using WorkBoard.Services.Abstraction.DTOs.Users;
using WorkBoard.Services.Abstraction.Hubs;
using WorkBoard.Services.Abstraction.Services;
using WorkBoard.Services.Abstraction.StateProviders;
using WorkBoard.Services.StateProviders;

namespace WorkBoard.UI.Components;

public partial class Sidebar : IDisposable
{
    [Inject]
    private ICurrentUserProvider CurrentUserProvider { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IBoardHubService BoardHubService { get; set; } = default!;

    [Inject]
    private IBoardService BoardService { get; set; } = default!;

    [Inject]
    private WorkspaceStateProvider WorkspaceStateProvider { get; set; } = default!;

    protected string SearchQuery { get; set; } = string.Empty;
    private bool _isProfileMenuOpen = false;

    private CancellationTokenSource? _searchCts;
    private bool _isSearching = false;
    private IReadOnlyList<BoardSearchResultDto>? _searchResults;

    protected override void OnInitialized()
    {
        CurrentUserProvider.OnProfileChanged += HandleProfileChanged;
        BoardHubService.OnUserAvatarUpdated += HandleUserAvatarUpdated;
    }

    private async Task HandleSearchInput(KeyboardEventArgs e)
    {
        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();
        var token = _searchCts.Token;

        if (string.IsNullOrWhiteSpace(SearchQuery) || SearchQuery.Length < 2)
        {
            _searchResults = null;
            _isSearching = false;
            return;
        }

        _isSearching = true;

        try
        {
            await Task.Delay(400, token);

            _searchResults = await BoardService.SearchBoardsAsync(SearchQuery, token);
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception)
        {
            _searchResults = Array.Empty<BoardSearchResultDto>();
        }
        finally
        {
            if (!token.IsCancellationRequested)
            {
                _isSearching = false;
                StateHasChanged();
            }
        }
    }

    private void SelectSearchResult(BoardSearchResultDto result)
    {
        SearchQuery = string.Empty;
        _searchResults = null;

        WorkspaceStateProvider.SetActiveWorkspace(
            result.WorkspaceId, 
            result.Role,
            result.SubscriptionTier);

        NavigationManager.NavigateTo($"/boards/{result.BoardId}");
    }

    private async Task HideSearchResults()
    {
        await Task.Delay(150);
        _searchResults = null;
        SearchQuery = string.Empty;
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
        _searchCts?.Cancel();
        _searchCts?.Dispose();
    }
}

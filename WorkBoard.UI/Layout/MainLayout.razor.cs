using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using MudBlazor;
using WorkBoard.Domain.Options;
using WorkBoard.Services.Abstraction.Hubs;
using WorkBoard.Services.Abstraction.StateProviders;
using WorkBoard.Services.StateProviders;

namespace WorkBoard.UI.Layout;

public partial class MainLayout : IAsyncDisposable
{
    [Inject]
    ICurrentUserProvider CurrentUserProvider { get; set; } = default!;

    [Inject]
    IOptions<WorkBoardUiOptions> UiOptions { get; set; } = default!;

    [Inject]
    IAppHubService AppHubService { get; set; } = default!;

    [Inject]
    BoardStateService BoardStateService { get; set; } = default!;

    [Inject]
    WorkspaceStateProvider WorkspaceStateService { get; set; } = default!;

    private MudTheme _customTheme = new MudTheme()
    {
    };

    private bool _isSidebarOpen = true;

    private void ToggleSidebar()
    {
        _isSidebarOpen = !_isSidebarOpen;
    }

    protected override async Task OnInitializedAsync()
    {
        AppHubService.OnSidebarBoardStatusChanged += HandleSidebarBoardStatusChanged;
        AppHubService.OnWorkspacesListUpdated += HandleWorkspacesListUpdated;

        try
        {
            await AppHubService.StartConnectionAsync(UiOptions.Value.BackendBaseUrl);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to start AppHub: {ex.Message}");
        }

        await CurrentUserProvider.LoadProfileAsync();
    }

    private void HandleSidebarBoardStatusChanged()
    {
        InvokeAsync(() =>
        {
            BoardStateService.NotifyBoardsListChanged();
            StateHasChanged();
        });
    }

    private void HandleWorkspacesListUpdated()
    {
        InvokeAsync(() =>
        {
            WorkspaceStateService.NotifyWorkspacesListChanged();
            StateHasChanged();
        });
    }

    public async ValueTask DisposeAsync()
    {
        AppHubService.OnSidebarBoardStatusChanged -= HandleSidebarBoardStatusChanged;
        AppHubService.OnWorkspacesListUpdated -= HandleWorkspacesListUpdated;

        await AppHubService.StopConnectionAsync();
    }
}

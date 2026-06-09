using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using WorkBoard.Services.Abstraction;
using WorkBoard.Services.Abstraction.DTOs;

namespace WorkBoard.UI.Components.Workspaces;

public partial class SidebarWorkspaces
{
    [Inject]
    private IWorkspaceService WorkspaceService { get; set; } = null!;

    protected IReadOnlyList<UserWorkspaceDto>? Workspaces { get; private set; }

    [CascadingParameter]
    private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

    protected Guid? SelectedWorkspaceId { get; private set; }

    protected bool _isDropdownOpen = true;

    protected bool _isCreateModalOpen;

    protected override async Task OnInitializedAsync()
    {
        if (AuthenticationStateTask is not null)
        {
            var authState = await AuthenticationStateTask;
            var user = authState?.User;

            if (user?.Identity is not { IsAuthenticated: true })
            {
                Workspaces = new List<UserWorkspaceDto>();
                return;
            }

            await LoadWorkspacesAsync();
        }
    }

    private async Task LoadWorkspacesAsync()
    {
        try
        {
            Workspaces = await WorkspaceService.GetUserWorkspacesAsync();
        }
        catch (Exception)
        {
            Workspaces = new List<UserWorkspaceDto>();
        }
    }

    protected void ToggleDropdown()
    {
        _isDropdownOpen = !_isDropdownOpen;
    }

    private void SelectWorkspace(Guid id)
    {
        SelectedWorkspaceId = id;
    }

    protected void OpenCreateModal()
    {
        _isCreateModalOpen = true;
    }

    protected void CloseCreateModal()
    {
        _isCreateModalOpen = false;
    }

    protected async Task HandleWorkspaceCreatedAsync(Guid newWorkspaceId)
    {
        _isCreateModalOpen = false;

        await LoadWorkspacesAsync();

        SelectWorkspace(newWorkspaceId);
    }
}

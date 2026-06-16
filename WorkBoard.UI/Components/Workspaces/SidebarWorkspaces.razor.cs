using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using WorkBoard.Services.Abstraction;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.StateProviders;

namespace WorkBoard.UI.Components.Workspaces;

public partial class SidebarWorkspaces
{
    [Inject]
    private IWorkspaceService WorkspaceService { get; set; } = null!;

    [Inject]
    private WorkspaceStateProvider WorkspaceStateProvider { get; set; } = null!;

    protected IReadOnlyList<UserWorkspaceDto>? Workspaces { get; private set; }

    [CascadingParameter]
    private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

    protected Guid? SelectedWorkspaceId { get; private set; }

    protected bool _isDropdownOpen = true;

    protected bool _isCreateModalOpen;

    protected bool _isDeleteModalOpen;
    protected UserWorkspaceDto? _workspaceToModify;

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

        var currentSpace = Workspaces?
            .FirstOrDefault(w => w.Id == id);

        WorkspaceStateProvider.SetActiveWorkspace(
            id, 
            currentSpace?.UserRole);
    }

    protected void OpenCreateModal()
    {
        _isCreateModalOpen = true;
    }

    protected void CloseCreateModal()
    {
        _isCreateModalOpen = false;
    }

    protected void OpenEditModal(UserWorkspaceDto workspace)
    {
        _workspaceToModify = workspace;
        _isCreateModalOpen = true;
    }

    protected void OpenDeleteModal(UserWorkspaceDto workspace)
    {
        _workspaceToModify = workspace;
        _isDeleteModalOpen = true;
    }

    protected void CloseDeleteModal()
    {
        _isDeleteModalOpen = false;
        _workspaceToModify = null;
    }

    protected async Task HandleWorkspaceSavedAsync(
        Guid workspaceId)
    {
        _isCreateModalOpen = false;
        _workspaceToModify = null;

        await LoadWorkspacesAsync();
        SelectWorkspace(workspaceId);
    }

    protected async Task HandleWorkspaceDeletedAsync()
    {
        if (_workspaceToModify != null && 
            SelectedWorkspaceId == _workspaceToModify.Id)
        {
            SelectedWorkspaceId = null;
            WorkspaceStateProvider.SetActiveWorkspace(null, null);
        }

        _isDeleteModalOpen = false;
        _workspaceToModify = null;

        await LoadWorkspacesAsync();
    }
}

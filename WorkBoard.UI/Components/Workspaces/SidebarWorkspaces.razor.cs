using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using WorkBoard.Domain.Enums;
using WorkBoard.Services.Abstraction.DTOs.Users;
using WorkBoard.Services.Abstraction.Services;
using WorkBoard.Services.Abstraction.StateProviders;
using WorkBoard.Services.StateProviders;

namespace WorkBoard.UI.Components.Workspaces;

public partial class SidebarWorkspaces : IDisposable
{
    [Inject]
    private IWorkspaceService WorkspaceService { get; set; } = null!;

    [Inject]
    private WorkspaceStateProvider WorkspaceStateProvider { get; set; } = null!;
    [Inject]
    private ICurrentUserProvider CurrentUserProvider { get; set; } = null!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    private BoardStateService BoardStateService { get; set; } = default!;

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
        WorkspaceStateProvider.OnWorkspaceChanged += HandleWorkspaceChanged;
        WorkspaceStateProvider.OnWorkspacesListChanged += HandleWorkspacesListChanged;

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

            SelectedWorkspaceId = WorkspaceStateProvider.SelectedWorkspaceId;

            if (SelectedWorkspaceId == null && Workspaces != null && Workspaces.Any())
            {
                SelectWorkspace(Workspaces.First().Id);
            }
        }
    }

    private void HandleWorkspaceChanged(
        Guid? workspaceId,
        WorkspaceRole? role,
        SubscriptionTier? tier)
    {
        SelectedWorkspaceId = workspaceId;
        InvokeAsync(StateHasChanged);
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

        var currentSpace = Workspaces?.FirstOrDefault(w => w.Id == id);

        WorkspaceStateProvider.SetActiveWorkspace(
            id,
            currentSpace?.UserRole,
            currentSpace?.SubscriptionTier);
    }

    protected void OpenCreateModal()
    {
        if (CurrentUserProvider.Profile?.SubscriptionTier == SubscriptionTier.Free)
        {
            var ownedWorkspacesCount = Workspaces?
                .Count(w => w.UserRole == WorkspaceRole.Owner) ?? 0;

            if (ownedWorkspacesCount >= 1)
            {
                Snackbar.Add(
                    "You can only create 1 workspace on the Free plan. " +
                    "Please upgrade to Pro to create more.", 
                    Severity.Warning);
                return;
            }
        }

        _workspaceToModify = null;
        _isCreateModalOpen = true;
    }

    protected void CloseCreateModal()
    {
        _workspaceToModify = null;
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
            WorkspaceStateProvider.SetActiveWorkspace(null, null, null);
        }

        _isDeleteModalOpen = false;
        _workspaceToModify = null;

        await LoadWorkspacesAsync();
    }

    private void HandleWorkspacesListChanged()
    {
        InvokeAsync(async () =>
        {
            await LoadWorkspacesAsync();

            if (SelectedWorkspaceId.HasValue &&
                Workspaces != null &&
                !Workspaces.Any(w => w.Id == SelectedWorkspaceId.Value))
            {
                SelectedWorkspaceId = null;
                WorkspaceStateProvider.SetActiveWorkspace(null, null, null);

                if (Workspaces.Any())
                {
                    SelectWorkspace(Workspaces.First().Id);
                }
            }
            else if (SelectedWorkspaceId == null &&
                    Workspaces != null && Workspaces.Any())
            {
                SelectWorkspace(Workspaces.First().Id);
            }
            else
            {
                var currentSpace = Workspaces!
                    .First(w => w.Id == SelectedWorkspaceId.Value);
                WorkspaceStateProvider.SetActiveWorkspace(
                    currentSpace.Id, 
                    currentSpace.UserRole, 
                    currentSpace.SubscriptionTier);

                BoardStateService.NotifyBoardsListChanged();
            }

            StateHasChanged();
        });
    }

    public void Dispose()
    {
        WorkspaceStateProvider.OnWorkspaceChanged -= HandleWorkspaceChanged;
        WorkspaceStateProvider.OnWorkspacesListChanged -= HandleWorkspacesListChanged;
    }
}

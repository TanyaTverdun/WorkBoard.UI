using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using MudBlazor;
using WorkBoard.Domain.Enums;
using WorkBoard.Domain.Options;
using WorkBoard.Services.Abstraction.DTOs.Users;
using WorkBoard.Services.Abstraction.DTOs.Workspaces;
using WorkBoard.Services.Abstraction.Hubs;
using WorkBoard.Services.Abstraction.Requests.Workspaces;
using WorkBoard.Services.Abstraction.Services;
using WorkBoard.Services.Abstraction.StateProviders;

namespace WorkBoard.UI.Pages;

public partial class RolesManagementPage : ComponentBase
{
    [Inject] 
    private ISnackbar Snackbar { get; set; } = null!;
    [Inject] 
    private IWorkspaceService WorkspaceService { get; set; } = null!;
    [Inject] 
    private ICurrentUserProvider CurrentUserProvider { get; set; } = null!;
    [Inject] 
    private IWorkspaceHubService WorkspaceHubService { get; set; } = null!;
    [Inject]
    private IOptions<WorkBoardUiOptions> UiOptions { get; set; } = null!;
    [Inject] 
    private IAppHubService AppHubService { get; set; } = null!;

    private bool _isLoading = true;
    private bool _isLoadingMembers = false;
    private bool _isWorkspaceMenuOpen;
    private UserSearchDto? _selectedUserToAdd;

    private bool _isInviteOpen;
    private WorkspaceRole _inviteRole = WorkspaceRole.Member;

    private Guid _currentUserId;

    private IReadOnlyList<UserWorkspaceDto> _workspaces = new List<UserWorkspaceDto>();
    private UserWorkspaceDto? _workspace;

    private List<WorkspaceMemberDto> _currentWorkspaceMembers = new();

    private bool IsCurrentUserObserver =>
        _currentWorkspaceMembers?.FirstOrDefault(m => m.IsCurrentUser)?.Role == WorkspaceRole.Observer;

    private string GetRoleLabel(WorkspaceRole role) => role switch
    {
        WorkspaceRole.Owner => "Owner",
        WorkspaceRole.Member => "Member",
        WorkspaceRole.Observer => "Observer",
        _ => "Unknown"
    };

    private string GetRoleSelectClass(WorkspaceRole role) => role switch
    {
        WorkspaceRole.Owner => "role-owner-color",
        WorkspaceRole.Member => "role-member-color",
        WorkspaceRole.Observer => "role-observer-color",
        _ => ""
    };

    protected override async Task OnInitializedAsync()
    {
        _isLoading = true;
        try
        {
            var userId = await CurrentUserProvider.GetUserIdAsync();
            if (userId.HasValue)
            {
                _currentUserId = userId.Value;
            }

            WorkspaceHubService.OnMemberAdded += HandleMemberAdded;
            WorkspaceHubService.OnMemberRemoved += HandleMemberRemoved;
            WorkspaceHubService.OnMemberRoleUpdated += HandleMemberRoleUpdated;
            AppHubService.OnWorkspacesListUpdated += HandleWorkspacesListUpdated;

            await LoadWorkspacesAsync();
        }
        catch (Exception)
        {
            Snackbar.Add("Failed to load workspaces", Severity.Error);
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task LoadWorkspacesAsync()
    {
        _workspaces = await WorkspaceService.GetWorkspacesForRoleManagementAsync();

        if (_workspaces.Any())
        {
            await SelectWorkspaceAsync(_workspaces.First());
        }
    }

    private void ToggleWorkspaceMenu()
    {
        _isWorkspaceMenuOpen = !_isWorkspaceMenuOpen;
    }

    private void CloseWorkspaceMenu()
    {
        _isWorkspaceMenuOpen = false;
    }

    private void SelectWorkspace(UserWorkspaceDto workspace)
    {
        _ = SelectWorkspaceAsync(workspace);
    }

    private async Task SelectWorkspaceAsync(UserWorkspaceDto workspace)
    {
        _isWorkspaceMenuOpen = false;

        if (_workspace != null && workspace.Id == _workspace.Id)
        {
            return;
        }

        if (_workspace != null)
        {
            await WorkspaceHubService.StopConnectionAsync(_workspace.Id);
        }

        _workspace = workspace;
        _currentWorkspaceMembers.Clear();

        await LoadWorkspaceMembersAsync(_workspace.Id);

        try
        {
            var backendUrl = UiOptions.Value.BackendBaseUrl;
            await WorkspaceHubService.StartConnectionAsync(
                backendUrl, 
                _workspace.Id);
        }
        catch (Exception)
        {
            Snackbar.Add(
                "Offline mode: live updates unavailable.", 
                Severity.Warning);
        }
    }

    private async Task LoadWorkspaceMembersAsync(Guid workspaceId)
    {
        _isLoadingMembers = true;
        StateHasChanged();

        try
        {
            var members = await WorkspaceService.GetWorkspaceMembersAsync(workspaceId);
            _currentWorkspaceMembers = members.ToList();
        }
        catch (Exception)
        {
            Snackbar.Add(
                "Failed to load workspace members", 
                Severity.Error);
        }
        finally
        {
            _isLoadingMembers = false;
            StateHasChanged();
        }
    }

    private void ToggleInvitePopup()
    {
        _isInviteOpen = !_isInviteOpen;
        if (_isInviteOpen)
        {
            _selectedUserToAdd = null;
            _inviteRole = WorkspaceRole.Member;
        }
    }

    private void CloseInvitePopup()
    {
        _isInviteOpen = false;
    }

    private async Task<IEnumerable<UserSearchDto>> SearchUsersAsync(
        string value, 
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(value) || 
            value.Length < 2 || 
            _workspace == null)
        {
            return new List<UserSearchDto>();
        }

        try
        {
            return await WorkspaceService.SearchAssignableUsersAsync(
                _workspace.Id, 
                value, cancellationToken);
        }
        catch (Exception)
        {
            return new List<UserSearchDto>();
        }
    }

    private async Task ConfirmInviteAsync()
    {
        if (_selectedUserToAdd == null || _workspace == null)
        {
            return;
        }

        try
        {
            var request = new AddWorkspaceMemberRequest
            {
                UserId = _selectedUserToAdd.UserId,
                Role = (int)_inviteRole
            };

            await WorkspaceService.AddWorkspaceMemberAsync(
                _workspace.Id, 
                request);

            _isInviteOpen = false;
            _selectedUserToAdd = null;
        }
        catch (Exception ex)
        {
            Snackbar.Add(
                $"Failed to add member: {ex.Message}", 
                Severity.Error);
        }
    }

    private bool CanEditRole(WorkspaceMemberDto member)
    {
        if (IsCurrentUserObserver)
        {
            return false;
        }

        return member.Role != WorkspaceRole.Owner && !member.IsCurrentUser;
    }

    private async Task OnRoleChanged(
        WorkspaceMemberDto member, 
        WorkspaceRole newRole)
    {
        if (!CanEditRole(member) || _workspace == null)
        {
            return;
        }

        try
        {
            var request = new UpdateWorkspaceMemberRoleRequest 
            { 
                NewRole = (int)newRole 
            };

            await WorkspaceService.UpdateWorkspaceMemberRoleAsync(
                _workspace.Id,
                member.Id,
                request);

            member.Role = newRole;
            Snackbar.Add(
                $"Role for {member.Name} updated to {GetRoleLabel(newRole)}", 
                Severity.Success);
        }
        catch (Exception)
        {
            Snackbar.Add("Failed to update role", Severity.Error);
        }
    }

    private Guid? _pendingDeleteUserId;

    private void PromptDelete(Guid memberId)
    {
        _pendingDeleteUserId = memberId;
    }

    private void CancelDelete()
    {
        _pendingDeleteUserId = null;
    }

    private async Task ConfirmDeleteAsync(WorkspaceMemberDto member)
    {
        if (_workspace == null)
        {
            return;
        }

        try
        {
            await WorkspaceService.RemoveWorkspaceMemberAsync(
                _workspace.Id, 
                member.Id);
        }
        catch (Exception ex)
        {
            Snackbar.Add(
                $"Failed to remove member: {ex.Message}",
                Severity.Error);
        }
        finally
        {
            _pendingDeleteUserId = null;
        }
    }

    private void HandleMemberAdded(WorkspaceMemberAddedDto data)
    {
        InvokeAsync(async () =>
        {
            if (data.UserId == _currentUserId)
            {
                Snackbar.Add(
                    "You have been added to a workspace!", 
                    Severity.Success);

                _workspaces = await WorkspaceService.GetWorkspacesForRoleManagementAsync();

                if (_workspace == null && _workspaces.Any())
                {
                    await SelectWorkspaceAsync(_workspaces.First());
                }
            }
            else
            {
                if (_workspace != null &&
                    !_currentWorkspaceMembers.Any(m => m.Id == data.UserId))
                {
                    var newMember = new WorkspaceMemberDto
                    {
                        Id = data.UserId,
                        Name = data.Name,
                        Email = data.Email,
                        Role = data.Role,
                        AvatarUrl = data.AvatarUrl,
                        AvatarColor = data.AvatarColor,
                        Initials = data.Initials,
                        IsCurrentUser = false
                    };

                    _currentWorkspaceMembers.Add(newMember);

                    _currentWorkspaceMembers = _currentWorkspaceMembers
                        .OrderBy(m => m.Role)
                        .ThenBy(m => m.Name)
                        .ToList();
                }
            }

            StateHasChanged();
        });
    }

    private void HandleMemberRoleUpdated(WorkspaceMemberRoleUpdatedDto data)
    {
        var member = _currentWorkspaceMembers.FirstOrDefault(
            m => m.Id == data.UserId);

        if (member != null && member.Role != data.NewRole)
        {
            member.Role = data.NewRole;
            InvokeAsync(StateHasChanged);
        }
    }

    private void HandleMemberRemoved(Guid userId)
    {
        var member = _currentWorkspaceMembers.FirstOrDefault(
            m => m.Id == userId);

        if (member == null)
        {
            return;
        }

        if (member.IsCurrentUser || userId == _currentUserId)
        {
            InvokeAsync(async () =>
            {
                Snackbar.Add(
                    "You have been removed from this workspace.", 
                    Severity.Warning);

                _workspace = null;
                _currentWorkspaceMembers.Clear();
                StateHasChanged();

                try
                {
                    _workspaces = await WorkspaceService.GetWorkspacesForRoleManagementAsync();

                    if (_workspaces.Any())
                    {
                        await SelectWorkspaceAsync(_workspaces.First());
                    }
                    else
                    {
                        StateHasChanged();
                    }
                }
                catch
                {
                    StateHasChanged();
                }
            });
        }
        else
        {
            _currentWorkspaceMembers.Remove(member);
            InvokeAsync(StateHasChanged);
        }
    }

    private void HandleWorkspacesListUpdated()
    {
        InvokeAsync(async () =>
        {
            _workspaces = await WorkspaceService.GetWorkspacesForRoleManagementAsync();

            if (_workspace != null)
            {
                if (!_workspaces.Any(w => w.Id == _workspace.Id))
                {
                    Snackbar.Add(
                        "Your access to the current workspace was revoked.", 
                        Severity.Warning);

                    _workspace = null;
                    _currentWorkspaceMembers.Clear();

                    if (_workspaces.Any())
                    {
                        await SelectWorkspaceAsync(_workspaces.First());
                    }
                    else
                    {
                        StateHasChanged();
                    }
                }
            }
            else if (_workspaces.Any())
            {
                await SelectWorkspaceAsync(_workspaces.First());
            }

            StateHasChanged();
        });
    }

    public async ValueTask DisposeAsync()
    {
        WorkspaceHubService.OnMemberAdded -= HandleMemberAdded;
        WorkspaceHubService.OnMemberRemoved -= HandleMemberRemoved;
        WorkspaceHubService.OnMemberRoleUpdated -= HandleMemberRoleUpdated;
        AppHubService.OnWorkspacesListUpdated -= HandleWorkspacesListUpdated;

        if (_workspace != null)
        {
            await WorkspaceHubService.StopConnectionAsync(_workspace.Id);
        }
    }
}
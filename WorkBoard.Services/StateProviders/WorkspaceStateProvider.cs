using WorkBoard.Domain.Enums;

namespace WorkBoard.Services.StateProviders;

public class WorkspaceStateProvider
{
    private Guid? _selectedWorkspaceId;
    private WorkspaceRole? _currentRole;

    public Guid? SelectedWorkspaceId => _selectedWorkspaceId;
    public WorkspaceRole? CurrentRole => _currentRole;

    public void SetActiveWorkspace(Guid? workspaceId, WorkspaceRole? role)
    {
        if (_selectedWorkspaceId != workspaceId || _currentRole != role)
        {
            _selectedWorkspaceId = workspaceId;
            _currentRole = role;

            OnWorkspaceChanged?.Invoke(workspaceId, role);
        }
    }

    public event Action<Guid?, WorkspaceRole?>? OnWorkspaceChanged;

    public event Action? OnWorkspacesListChanged;

    public void NotifyWorkspacesListChanged()
    {
        OnWorkspacesListChanged?.Invoke();
    }
}

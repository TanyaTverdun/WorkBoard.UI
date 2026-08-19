using WorkBoard.Domain.Enums;

namespace WorkBoard.Services.StateProviders;

public class WorkspaceStateProvider
{
    private Guid? _selectedWorkspaceId;
    private WorkspaceRole? _currentRole;
    private SubscriptionTier? _currentWorkspaceTier;

    public Guid? SelectedWorkspaceId => _selectedWorkspaceId;
    public WorkspaceRole? CurrentRole => _currentRole;
    public SubscriptionTier? CurrentWorkspaceTier => _currentWorkspaceTier;

    public event Action<Guid?, WorkspaceRole?, SubscriptionTier?>? OnWorkspaceChanged;

    public void SetActiveWorkspace(
        Guid? workspaceId,
        WorkspaceRole? role,
        SubscriptionTier? tier)
    {
        bool isDowngrade = _selectedWorkspaceId != null &&
                           _selectedWorkspaceId == workspaceId &&
                           _currentWorkspaceTier == SubscriptionTier.Pro &&
                           tier == SubscriptionTier.Free;

        if (_selectedWorkspaceId != workspaceId || 
            _currentRole != role || 
            _currentWorkspaceTier != tier)
        {
            _selectedWorkspaceId = workspaceId;
            _currentRole = role;
            _currentWorkspaceTier = tier;

            OnWorkspaceChanged?.Invoke(workspaceId, role, tier);

            if (isDowngrade)
            {
                OnWorkspaceDowngraded?.Invoke();
            }
        }
    }

    public event Action? OnWorkspacesListChanged;
    public event Action? OnWorkspaceDowngraded;

    public void NotifyWorkspacesListChanged()
    {
        OnWorkspacesListChanged?.Invoke();
    }
}

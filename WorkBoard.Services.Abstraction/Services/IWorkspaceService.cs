using WorkBoard.Services.Abstraction.DTOs.Users;
using WorkBoard.Services.Abstraction.DTOs.Workspaces;
using WorkBoard.Services.Abstraction.Requests.Workspaces;

namespace WorkBoard.Services.Abstraction.Services;

public interface IWorkspaceService
{
    Task<IReadOnlyList<UserWorkspaceDto>> GetUserWorkspacesAsync(
        CancellationToken cancellationToken = default);

    Task<Guid> CreateWorkspaceAsync(
        CreateWorkspaceRequest request, 
        CancellationToken cancellationToken = default);

    Task UpdateWorkspaceAsync(
        Guid id,
        UpdateWorkspaceRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteWorkspaceAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UserWorkspaceDto>> GetWorkspacesForRoleManagementAsync(
        CancellationToken cancellationToken = default);

    Task UpdateWorkspaceMemberRoleAsync(
        Guid workspaceId,
        Guid memberId,
        UpdateWorkspaceMemberRoleRequest request,
        CancellationToken cancellationToken = default);

    Task AddWorkspaceMemberAsync(
        Guid workspaceId,
        AddWorkspaceMemberRequest request,
        CancellationToken cancellationToken = default);

    Task RemoveWorkspaceMemberAsync(
        Guid workspaceId,
        Guid memberId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WorkspaceMemberDto>> GetWorkspaceMembersAsync(
        Guid workspaceId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UserSearchDto>> SearchAssignableUsersAsync(
        Guid workspaceId,
        string searchTerm,
        CancellationToken cancellationToken = default);
}

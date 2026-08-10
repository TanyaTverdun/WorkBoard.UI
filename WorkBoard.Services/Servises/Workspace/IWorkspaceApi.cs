using Refit;
using WorkBoard.Services.Abstraction.DTOs.Users;
using WorkBoard.Services.Abstraction.Requests.Workspaces;

namespace WorkBoard.Services.Servises.Workspace;

internal interface IWorkspaceApi
{
    [Get("/api/workspaces")]
    Task<IReadOnlyList<UserWorkspaceDto>> GetMyWorkspacesAsync(
        CancellationToken cancellationToken = default);

    [Post("/api/workspaces")]
    Task<Guid> CreateWorkspaceAsync(
        [Body] CreateWorkspaceRequest request, 
        CancellationToken cancellationToken = default);

    [Put("/api/workspaces/{id}")]
    Task UpdateWorkspaceAsync(
        Guid id,
        [Body] UpdateWorkspaceRequest request,
        CancellationToken cancellationToken = default);

    [Delete("/api/workspaces/{id}")]
    Task DeleteWorkspaceAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    [Get("/api/workspaces/UserWorkspaces")]
    Task<IReadOnlyList<UserWorkspaceDto>> GetWorkspacesForRoleManagementAsync(
        CancellationToken cancellationToken = default);

    [Put("/api/workspaces/{id}/members/{memberId}/role")]
    Task UpdateWorkspaceMemberRoleAsync(
        Guid id,
        Guid memberId,
        [Body] UpdateWorkspaceMemberRoleRequest request,
        CancellationToken cancellationToken = default);

    [Post("/api/workspaces/{id}/members")]
    Task AddWorkspaceMemberAsync(
        Guid id,
        [Body] AddWorkspaceMemberRequest request,
        CancellationToken cancellationToken = default);

    [Delete("/api/workspaces/{id}/members/{memberId}")]
    Task RemoveWorkspaceMemberAsync(
        Guid id,
        Guid memberId,
        CancellationToken cancellationToken = default);
}

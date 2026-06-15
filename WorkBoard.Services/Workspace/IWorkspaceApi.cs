using Refit;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Requests;

namespace WorkBoard.Services.Workspace;

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
}

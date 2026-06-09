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
}

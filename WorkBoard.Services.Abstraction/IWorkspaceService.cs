using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Requests;

namespace WorkBoard.Services.Abstraction;

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
}

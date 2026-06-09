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
}

using WorkBoard.Services.Abstraction.DTOs.Workspaces;

namespace WorkBoard.Services.Abstraction.Hubs;

public interface IWorkspaceHubService : IAsyncDisposable
{
    event Action<WorkspaceMemberAddedDto>? OnMemberAdded;
    event Action<Guid>? OnMemberRemoved;
    event Action<WorkspaceMemberRoleUpdatedDto>? OnMemberRoleUpdated;

    Task StartConnectionAsync(
        string backendUrl,
        Guid workspaceId,
        CancellationToken cancellationToken = default);

    Task StopConnectionAsync(
        Guid workspaceId,
        CancellationToken cancellationToken = default);
}

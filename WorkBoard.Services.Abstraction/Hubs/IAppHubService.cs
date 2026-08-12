namespace WorkBoard.Services.Abstraction.Hubs;

public interface IAppHubService : IAsyncDisposable
{
    event Action? OnSidebarBoardStatusChanged;
    event Action? OnWorkspacesListUpdated;

    Task StartConnectionAsync(
        string backendUrl, 
        CancellationToken cancellationToken = default);

    Task StopConnectionAsync(
        CancellationToken cancellationToken = default);
}

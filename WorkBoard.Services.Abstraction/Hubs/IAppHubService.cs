namespace WorkBoard.Services.Abstraction.Hubs;

public interface IAppHubService : IAsyncDisposable
{
    event Action? OnSidebarBoardStatusChanged;

    Task StartConnectionAsync(
        string backendUrl, 
        CancellationToken cancellationToken = default);

    Task StopConnectionAsync(
        CancellationToken cancellationToken = default);
}

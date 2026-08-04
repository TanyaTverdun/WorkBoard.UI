using WorkBoard.Domain.Enums;

namespace WorkBoard.Services.Abstraction.Hubs;

public interface IArchivationHubService : IAsyncDisposable
{
    event Action<Guid, BoardArchiveStatus>? OnArchivationStatusChanged;

    Task StartConnectionAsync(
        string backendUrl,
        CancellationToken cancellationToken = default);

    Task StopConnectionAsync(
        CancellationToken cancellationToken = default);
}

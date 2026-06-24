using WorkBoard.Services.Abstraction.DTOs;

namespace WorkBoard.Services.Abstraction.Hubs;

public interface IBoardHubService : IAsyncDisposable
{
    event Action<CardDto>? OnCardCreated;

    event Action<SectionDto>? OnSectionCreated;

    event Action<SectionRenameDto>? OnSectionRenamed;


    Task StartConnectionAsync(
        string backendUrl,
        Guid boardId,
        CancellationToken cancellationToken = default);

    Task StopConnectionAsync(
        Guid boardId,
        CancellationToken cancellationToken = default);
}

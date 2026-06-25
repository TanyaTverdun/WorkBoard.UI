using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Hubs;

namespace WorkBoard.Services.Hubs;

internal class BoardHubService : IBoardHubService
{
    private HubConnection? _hubConnection;
    private readonly ILogger<BoardHubService> _logger;
    private readonly IAccessTokenProvider _tokenProvider;

    public event Action<CardDto>? OnCardCreated;
    public event Action<SectionDto>? OnSectionCreated;
    public event Action<SectionRenameDto>? OnSectionRenamed;
    public event Action<Guid>? OnSectionDeleted;

    public BoardHubService(
        ILogger<BoardHubService> logger,
        IAccessTokenProvider tokenProvider)
    {
        _logger = logger;
        _tokenProvider = tokenProvider;
    }

    public async Task StartConnectionAsync(
        string backendUrl,
        Guid boardId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl($"{backendUrl}/hubs/board", options =>
                {
                    options.AccessTokenProvider = async () =>
                    {
                        var tokenResult = await _tokenProvider
                            .RequestAccessToken();

                        if (tokenResult.TryGetToken(out var token))
                        {
                            return token.Value;
                        }

                        return null;
                    };
                })
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<CardDto>("CardCreated", (card) =>
            {
                _logger.LogInformation(
                    "Received new card via SignalR: {Title}", card.Title);
                OnCardCreated?.Invoke(card);
            });

            _hubConnection.On<SectionDto>("SectionCreated", (section) =>
            {
                _logger.LogInformation(
                    "Received new section via SignalR: {Name}", section.Name);
                OnSectionCreated?.Invoke(section);
            });

            _hubConnection.On<SectionRenameDto>("SectionRenamed", (section) =>
            {
                _logger.LogInformation(
                    "Section renamed: {NewName} (ID: {SectionId})", 
                    section.NewName, 
                    section.SectionId);

                OnSectionRenamed?.Invoke(section);
            });

            _hubConnection.On<Guid>("SectionDeleted", (sectionId) =>
            {
                _logger.LogInformation("Section deleted: {SectionId}", sectionId);
                OnSectionDeleted?.Invoke(sectionId);
            });

            await _hubConnection.StartAsync(cancellationToken);

            await _hubConnection.InvokeAsync(
                "JoinBoard",
                boardId.ToString(),
                cancellationToken);

            _logger.LogInformation(
                "Successfully connected to SignalR for Board: {BoardId}",
                boardId);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Error connecting to SignalR hub.",
                ex);
        }
    }

    public async Task StopConnectionAsync(
        Guid boardId,
        CancellationToken cancellationToken = default)
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.InvokeAsync(
                "LeaveBoard",
                boardId.ToString(),
                cancellationToken);

            await _hubConnection.StopAsync(cancellationToken);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}

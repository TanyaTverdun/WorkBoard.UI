using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using WorkBoard.Domain.Constants;
using WorkBoard.Domain.Enums;
using WorkBoard.Services.Abstraction.Hubs;

namespace WorkBoard.Services.Hubs;

internal class ArchivationHubService : IArchivationHubService
{
    private HubConnection? _hubConnection;
    private readonly ILogger<ArchivationHubService> _logger;
    private readonly IAccessTokenProvider _tokenProvider;

    public event Action<Guid, BoardArchiveStatus>? OnArchivationStatusChanged;

    public ArchivationHubService(
        ILogger<ArchivationHubService> logger,
        IAccessTokenProvider tokenProvider)
    {
        _logger = logger;
        _tokenProvider = tokenProvider;
    }

    public async Task StartConnectionAsync(
        string backendUrl,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl($"{backendUrl}/hubs/archivation", options =>
                {
                    options.AccessTokenProvider = async () =>
                    {
                        var tokenResult = await _tokenProvider.RequestAccessToken();

                        if (tokenResult.TryGetToken(out var token))
                        {
                            return token.Value;
                        }

                        return null;
                    };
                })
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<Guid, BoardArchiveStatus>(
                ArchivationHubEvents.ArchivationStatusChanged,
                (boardId, newStatus) =>
                {
                    _logger.LogInformation(
                    "Archivation status changed via SignalR " +
                    "for board {BoardId} to {Status}",
                    boardId,
                    newStatus);

                    OnArchivationStatusChanged?.Invoke(boardId, newStatus);
                });

            await _hubConnection.StartAsync(cancellationToken);

            _logger.LogInformation("Successfully connected " +
                "to Archivation SignalR Hub.");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Error connecting to Archivation SignalR hub.",
                ex);
        }
    }

    public async Task StopConnectionAsync(
        CancellationToken cancellationToken = default)
    {
        if (_hubConnection is not null)
        {
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

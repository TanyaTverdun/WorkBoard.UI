using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using WorkBoard.Domain.Constants;
using WorkBoard.Services.Abstraction.Hubs;

namespace WorkBoard.Services.Hubs;

internal class AppHubService : IAppHubService
{
    private HubConnection? _hubConnection;
    private readonly ILogger<AppHubService> _logger;
    private readonly IAccessTokenProvider _tokenProvider;

    public event Action? OnSidebarBoardStatusChanged;

    public AppHubService(
        ILogger<AppHubService> logger,
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
                .WithUrl($"{backendUrl}/hubs/app", options =>
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

            _hubConnection.On(
                AppHubEvents.SidebarBoardChanged,
                () =>
                {
                    _logger.LogInformation(
                        "Global AppHub event: Board {BoardId} changed status to {Status}");

                    OnSidebarBoardStatusChanged?.Invoke();
                });

            await _hubConnection.StartAsync(cancellationToken);
            _logger.LogInformation("Successfully connected to Global AppHub.");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error connecting to Global AppHub.", ex);
        }
    }

    public async Task StopConnectionAsync(CancellationToken cancellationToken = default)
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

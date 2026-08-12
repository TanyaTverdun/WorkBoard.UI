using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using WorkBoard.Domain.Constants;
using WorkBoard.Services.Abstraction.DTOs.Workspaces;
using WorkBoard.Services.Abstraction.Hubs;

namespace WorkBoard.Services.Hubs;

internal class WorkspaceHubService : IWorkspaceHubService
{
    private HubConnection? _hubConnection;
    private readonly ILogger<WorkspaceHubService> _logger;
    private readonly IAccessTokenProvider _tokenProvider;

    public event Action<WorkspaceMemberAddedDto>? OnMemberAdded;
    public event Action<Guid>? OnMemberRemoved;
    public event Action<WorkspaceMemberRoleUpdatedDto>? OnMemberRoleUpdated;

    public WorkspaceHubService(
        ILogger<WorkspaceHubService> logger,
        IAccessTokenProvider tokenProvider)
    {
        _logger = logger;
        _tokenProvider = tokenProvider;
    }

    public async Task StartConnectionAsync(
        string backendUrl,
        Guid workspaceId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl($"{backendUrl}/hubs/workspace", options =>
                {
                    options.AccessTokenProvider = async () =>
                    {
                        var tokenResult = await _tokenProvider.RequestAccessToken();
                        return tokenResult.TryGetToken(out var token) ? token.Value : null;
                    };
                })
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<WorkspaceMemberAddedDto>(
                WorkspaceHubEvents.MemberAdded,
                (data) =>
                {
                    _logger.LogInformation(
                        "SignalR: Member added to workspace: {UserId}", 
                        data.UserId);
                    OnMemberAdded?.Invoke(data);
                });

            _hubConnection.On<Guid>(
                WorkspaceHubEvents.MemberRemoved,
                (userId) =>
                {
                    _logger.LogInformation(
                        "SignalR: Member removed from workspace: {UserId}",
                        userId);
                    OnMemberRemoved?.Invoke(userId);
                });

            _hubConnection.On<WorkspaceMemberRoleUpdatedDto>(
                WorkspaceHubEvents.MemberRoleUpdated,
                (data) =>
                {
                    _logger.LogInformation(
                        "SignalR: Member role updated: {UserId} to {Role}",
                        data.UserId,
                        data.NewRole);
                    OnMemberRoleUpdated?.Invoke(data);
                });

            await _hubConnection.StartAsync(cancellationToken);

            await _hubConnection.InvokeAsync(
                WorkspaceHubEvents.JoinWorkspace,
                workspaceId.ToString(),
                cancellationToken);

            _logger.LogInformation(
                "Successfully connected to SignalR for Workspace: {WorkspaceId}",
                workspaceId);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Error connecting to Workspace SignalR hub.", 
                ex);
        }
    }

    public async Task StopConnectionAsync(
        Guid workspaceId,
        CancellationToken cancellationToken = default)
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.InvokeAsync(
                WorkspaceHubEvents.LeaveWorkspace,
                workspaceId.ToString(),
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

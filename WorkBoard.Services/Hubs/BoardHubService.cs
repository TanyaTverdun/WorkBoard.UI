using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using WorkBoard.Domain.Constants;
using WorkBoard.Domain.Enums;
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
    public event Action<Guid, double>? OnSectionMoved;
    public event Action<Guid, BoardRole>? OnMemberRoleUpdated;
    public event Action<Guid>? OnMemberRemoved;
    public event Action<Guid, Guid, double>? OnCardMoved;

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

            _hubConnection.On<CardDto>(BoardHubEvents.CardCreated, (card) =>
            {
                _logger.LogInformation(
                    "Received new card via SignalR: {Title}", card.Title);
                OnCardCreated?.Invoke(card);
            });

            _hubConnection.On<SectionDto>(BoardHubEvents.SectionCreated, (section) =>
            {
                _logger.LogInformation(
                    "Received new section via SignalR: {Name}", section.Name);
                OnSectionCreated?.Invoke(section);
            });

            _hubConnection.On<SectionRenameDto>(BoardHubEvents.SectionRenamed, (section) =>
            {
                _logger.LogInformation(
                    "Section renamed: {NewName} (ID: {SectionId})", 
                    section.NewName, 
                    section.SectionId);

                OnSectionRenamed?.Invoke(section);
            });

            _hubConnection.On<Guid>(BoardHubEvents.SectionDeleted, (sectionId) =>
            {
                _logger.LogInformation("Section deleted: {SectionId}", sectionId);
                OnSectionDeleted?.Invoke(sectionId);
            });

            _hubConnection.On<SectionMoveDto>(BoardHubEvents.SectionMoved, (data) =>
            {
                _logger.LogInformation(
                    "Section moved: {SectionId} to {NewPosition}", 
                    data.SectionId, 
                    data.NewPosition);

                OnSectionMoved?.Invoke(data.SectionId, data.NewPosition);
            });

            _hubConnection.On<MemberRoleUpdatedDto>(BoardHubEvents.MemberRoleUpdated, (data) =>
            {
                _logger.LogInformation(
                    "Member role updated: {UserId} to {NewRole}", 
                    data.UserId, 
                    data.NewRole);

                OnMemberRoleUpdated?.Invoke(data.UserId, data.NewRole);
            });

            _hubConnection.On<Guid>(BoardHubEvents.MemberRemoved, (userId) =>
            {
                _logger.LogInformation("Member removed from board: {UserId}", userId);
                OnMemberRemoved?.Invoke(userId);
            });

            _hubConnection.On<Guid, Guid, double>(BoardHubEvents.CardMoved, (cardId, newSectionId, newPosition) =>
            {
                _logger.LogInformation(
                    "Card moved: {CardId} to section {NewSectionId} at {NewPosition}",
                    cardId,
                    newSectionId,
                    newPosition);

                OnCardMoved?.Invoke(cardId, newSectionId, newPosition);
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
                BoardHubEvents.LeaveBoard,
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

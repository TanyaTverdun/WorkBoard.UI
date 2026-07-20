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
    public event Action<Guid>? OnCardDeleted;
    public event Action<CardRenameDto>? OnCardRenamed;
    public event Action<CommentDto>? OnCommentAdded;
    public event Action<ActivityLogDto>? OnActivityLogAdded;
    public event Action<CardDueDateUpdateDto>? OnCardDueDateUpdated;
    public event Action<Guid, LabelDto>? OnLabelAddedToCard;
    public event Action<Guid, Guid>? OnLabelRemovedFromCard;
    public event Action<LabelDto>? OnLabelCreated;
    public event Action<LabelDto>? OnLabelUpdated;
    public event Action<Guid>? OnLabelDeleted;
    public event Action<AssigneeAddDto>? OnAssigneeAdded;
    public event Action<AssigneeRemoveDto>? OnAssigneeRemoved;
    public event Action<CardMovedDto>? OnCardMoved;
    public event Action<CardDescriptionUpdateDto>? OnCardDescriptionUpdated;
    public event Action<ChecklistItemAddedDto>? OnChecklistItemAdded;
    public event Action<ChecklistCreatedDto>? OnChecklistCreated;
    public event Action<ChecklistDeletedDto>? OnChecklistDeleted;
    public event Action<ChecklistItemDeletedDto>? OnChecklistItemDeleted;
    public event Action<ChecklistRenamedDto>? OnChecklistRenamed;
    public event Action<ChecklistItemRenamedDto>? OnChecklistItemRenamed;
    public event Action<ChecklistItemStatusUpdatedDto>? OnChecklistItemStatusUpdated;
    public event Action<AttachmentAddedDto>? OnAttachmentAdded;
    public event Action<AttachmentDeletedDto>? OnAttachmentDeleted;

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

            _hubConnection.On<CardDto>(
                BoardHubEvents.CardCreated, 
                (card) =>
            {
                _logger.LogInformation(
                    "Received new card via SignalR: {Title}", 
                    card.Title);
                OnCardCreated?.Invoke(card);
            });

            _hubConnection.On<SectionDto>(
                BoardHubEvents.SectionCreated, 
                (section) =>
            {
                _logger.LogInformation(
                    "Received new section via SignalR: {Name}", 
                    section.Name);
                OnSectionCreated?.Invoke(section);
            });

            _hubConnection.On<SectionRenameDto>(
                BoardHubEvents.SectionRenamed, 
                (section) =>
            {
                _logger.LogInformation(
                    "Section renamed: {NewName} (ID: {SectionId})", 
                    section.NewName, 
                    section.SectionId);

                OnSectionRenamed?.Invoke(section);
            });

            _hubConnection.On<Guid>(
                BoardHubEvents.SectionDeleted, 
                (sectionId) =>
            {
                _logger.LogInformation(
                    "Section deleted: {SectionId}", 
                    sectionId);
                OnSectionDeleted?.Invoke(sectionId);
            });

            _hubConnection.On<SectionMoveDto>(
                BoardHubEvents.SectionMoved, 
                (data) =>
            {
                _logger.LogInformation(
                    "Section moved: {SectionId} to {NewPosition}", 
                    data.SectionId, 
                    data.NewPosition);

                OnSectionMoved?.Invoke(data.SectionId, data.NewPosition);
            });

            _hubConnection.On<MemberRoleUpdatedDto>(
                BoardHubEvents.MemberRoleUpdated, 
                (data) =>
            {
                _logger.LogInformation(
                    "Member role updated: {UserId} to {NewRole}", 
                    data.UserId, 
                    data.NewRole);

                OnMemberRoleUpdated?.Invoke(data.UserId, data.NewRole);
            });

            _hubConnection.On<Guid>(
                BoardHubEvents.MemberRemoved, 
                (userId) =>
            {
                _logger.LogInformation(
                    "Member removed from board: {UserId}", 
                    userId);
                OnMemberRemoved?.Invoke(userId);
            });

            _hubConnection.On<Guid>(
                BoardHubEvents.CardDeleted, 
                (cardId) =>
            {
                _logger.LogInformation(
                    "Card deleted via SignalR: {CardId}", 
                    cardId);
                OnCardDeleted?.Invoke(cardId);
            });

            _hubConnection.On<CardRenameDto>(
                BoardHubEvents.CardRenamed, 
                (data) =>
            {
                _logger.LogInformation(
                    "Card renamed: {NewTitle} (ID: {CardId})", 
                    data.NewTitle, 
                    data.CardId);

                OnCardRenamed?.Invoke(data);
            });

            _hubConnection.On<CommentDto>(
                BoardHubEvents.CommentAdded, 
                (comment) =>
            {
                _logger.LogInformation(
                    "Received new comment via SignalR for card: {CardId}", 
                    comment.CardId);

                OnCommentAdded?.Invoke(comment);
            });

            _hubConnection.On<ActivityLogDto>(
                BoardHubEvents.ActivityLogAdded, 
                (log) =>
            {
                _logger.LogInformation(
                    "Received new activity log via SignalR for card: {CardId}",
                    log.CardId);

                OnActivityLogAdded?.Invoke(log);
            });

            _hubConnection.On<CardDueDateUpdateDto>(
                BoardHubEvents.CardDueDateUpdated, 
                (data) =>
            {
                _logger.LogInformation(
                    "Card due date updated: {CardId} to {DueDate}",
                    data.CardId,
                    data.DueDate);

                OnCardDueDateUpdated?.Invoke(data);
            });

            _hubConnection.On<LabelAddDto>(
                BoardHubEvents.LabelAddedToCard, 
                (data) =>
            {
                _logger.LogInformation(
                    "Received label attached to card via SignalR: " +
                    "Label {LabelId} to Card {CardId}",
                    data.Label.Id,
                    data.CardId);

                OnLabelAddedToCard?.Invoke(data.CardId, data.Label);
            });

            _hubConnection.On<LabelRemoveDto>(
                BoardHubEvents.LabelRemovedFromCard, 
                (data) =>
            {
                _logger.LogInformation(
                    "Received label removed from card via SignalR: " +
                    "Label {LabelId} from Card {CardId}",
                    data.LabelId,
                    data.CardId);

                OnLabelRemovedFromCard?.Invoke(data.CardId, data.LabelId);
            });

            _hubConnection.On<LabelDto>(
                BoardHubEvents.LabelCreated, 
                (label) =>
            {
                _logger.LogInformation(
                    "Received new label created via SignalR: " +
                    "{LabelName} (ID: {LabelId})",
                    label.Name,
                    label.Id);

                OnLabelCreated?.Invoke(label);
            });

            _hubConnection.On<LabelDto>(
                BoardHubEvents.LabelUpdated, 
                (label) =>
            {
                _logger.LogInformation(
                    "Received label updated via SignalR: " +
                    "{LabelName} (ID: {LabelId})",
                    label.Name,
                    label.Id);

                OnLabelUpdated?.Invoke(label);
            });

            _hubConnection.On<Guid>(
                BoardHubEvents.LabelDeleted, 
                (labelId) =>
            {
                _logger.LogInformation(
                    "Received label deleted via SignalR: {LabelId}", 
                    labelId);

                OnLabelDeleted?.Invoke(labelId);
            });

            _hubConnection.On<AssigneeAddDto>(
                BoardHubEvents.AssigneeAdded, 
                (data) =>
            {
                _logger.LogInformation(
                    "Received new assignee via SignalR for card: {CardId}",
                    data.CardId);

                OnAssigneeAdded?.Invoke(data);
            });

            _hubConnection.On<AssigneeRemoveDto>(
                BoardHubEvents.AssigneeRemoved, 
                (data) =>
            {
                _logger.LogInformation(
                    "Received assignee removed via SignalR for card: " +
                    "{CardId}, User: {UserId}",
                    data.CardId,
                    data.UserId);

                OnAssigneeRemoved?.Invoke(data);
            });

            _hubConnection.On<CardMovedDto>(
                BoardHubEvents.CardMoved, 
                (data) =>
            {
                _logger.LogInformation(
                    "Received card moved via SignalR: " +
                    "Card {CardId} to Section {SectionName}",
                    data.CardId,
                    data.NewSectionName);

                OnCardMoved?.Invoke(data);
            });

            _hubConnection.On<CardDescriptionUpdateDto>(
                BoardHubEvents.CardDescriptionUpdated, 
                (data) =>
            {
                _logger.LogInformation(
                    "Received card description updated via SignalR " +
                    "for card: {CardId}",
                    data.CardId);

                OnCardDescriptionUpdated?.Invoke(data);
            });

            _hubConnection.On<ChecklistItemAddedDto>(
                BoardHubEvents.ChecklistItemAdded, 
                (data) =>
            {
                _logger.LogInformation(
                    "Received checklist item added via SignalR " +
                    "for checklist: {ChecklistId}",
                    data.ChecklistId);

                OnChecklistItemAdded?.Invoke(data);
            });

            _hubConnection.On<ChecklistCreatedDto>(
                BoardHubEvents.ChecklistCreated, 
                (data) =>
            {
                _logger.LogInformation(
                    "Received checklist created via SignalR " +
                    "for card: {CardId}",
                    data.CardId);

                OnChecklistCreated?.Invoke(data);
            });

            _hubConnection.On<ChecklistDeletedDto>(
                BoardHubEvents.ChecklistDeleted, 
                (data) =>
            {
                _logger.LogInformation(
                    "Received checklist deleted via SignalR " +
                    "for checklist: {ChecklistId}",
                    data.ChecklistId);

                OnChecklistDeleted?.Invoke(data);
            });

            _hubConnection.On<ChecklistItemDeletedDto>(
                BoardHubEvents.ChecklistItemDeleted, 
                (data) =>
            {
                _logger.LogInformation(
                    "Received checklist item deleted via SignalR " +
                    "for item: {ItemId}",
                    data.ItemId);

                OnChecklistItemDeleted?.Invoke(data);
            });

            _hubConnection.On<ChecklistRenamedDto>(
                BoardHubEvents.ChecklistRenamed, 
                (data) =>
            {
                _logger.LogInformation(
                    "Received checklist renamed: {ChecklistId}", 
                    data.ChecklistId);

                OnChecklistRenamed?.Invoke(data);
            });

            _hubConnection.On<ChecklistItemRenamedDto>(
                BoardHubEvents.ChecklistItemRenamed, 
                (data) =>
            {
                _logger.LogInformation(
                    "Received checklist item renamed via SignalR: " +
                    "Item {ItemId} in Checklist {ChecklistId} to '{NewTitle}'",
                    data.ItemId,
                    data.ChecklistId,
                    data.NewTitle);

                OnChecklistItemRenamed?.Invoke(data);
            });

            _hubConnection.On<ChecklistItemStatusUpdatedDto>(
                BoardHubEvents.ChecklistItemStatusUpdated, 
                (data) =>
            {
                _logger.LogInformation(
                    "Received checklist item status update: " +
                    "Item {ItemId} to {IsDone}",
                    data.ItemId,
                    data.IsDone);

                OnChecklistItemStatusUpdated?.Invoke(data);
            });

            _hubConnection.On<AttachmentAddedDto>(
                BoardHubEvents.AttachmentAdded, 
                (data) =>
            {
                _logger.LogInformation(
                    "Received new attachment via SignalR for Card: " +
                    "{CardId}, File: {FileName}",
                    data.CardId,
                    data.Attachment.FileName);

                OnAttachmentAdded?.Invoke(data);
            });

            _hubConnection.On<AttachmentDeletedDto>(
                BoardHubEvents.AttachmentDeleted, 
                (data) =>
            {
                _logger.LogInformation(
                    "Received attachment deleted via SignalR for Card: " +
                    "{CardId}, Attachment: {AttachmentId}",
                    data.CardId,
                    data.AttachmentId);

                OnAttachmentDeleted?.Invoke(data);
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

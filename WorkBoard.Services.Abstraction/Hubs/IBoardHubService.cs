using WorkBoard.Domain.Enums;
using WorkBoard.Services.Abstraction.DTOs;

namespace WorkBoard.Services.Abstraction.Hubs;

public interface IBoardHubService : IAsyncDisposable
{
    event Action<CardDto>? OnCardCreated;

    event Action<SectionDto>? OnSectionCreated;

    event Action<SectionRenameDto>? OnSectionRenamed;

    event Action<Guid>? OnSectionDeleted;

    event Action<Guid, double>? OnSectionMoved;

    event Action<Guid, BoardRole>? OnMemberRoleUpdated;

    event Action<Guid>? OnMemberRemoved;

    event Action<Guid>? OnCardDeleted;

    event Action<CardRenameDto>? OnCardRenamed;

    event Action<CommentDto>? OnCommentAdded;

    event Action<ActivityLogDto>? OnActivityLogAdded;

    event Action<CardDueDateUpdateDto>? OnCardDueDateUpdated;

    event Action<Guid, LabelDto>? OnLabelAddedToCard;

    event Action<Guid, Guid>? OnLabelRemovedFromCard;

    event Action<LabelDto>? OnLabelCreated;

    event Action<LabelDto>? OnLabelUpdated;

    event Action<Guid>? OnLabelDeleted;

    event Action<AssigneeAddDto>? OnAssigneeAdded;

    event Action<AssigneeRemoveDto>? OnAssigneeRemoved;

    event Action<CardMovedDto>? OnCardMoved;

    event Action<CardDescriptionUpdateDto>? OnCardDescriptionUpdated;

    event Action<ChecklistItemAddedDto>? OnChecklistItemAdded;

    event Action<ChecklistCreatedDto>? OnChecklistCreated;

    event Action<ChecklistDeletedDto>? OnChecklistDeleted;

    event Action<ChecklistItemDeletedDto>? OnChecklistItemDeleted;

    event Action<ChecklistRenamedDto>? OnChecklistRenamed;

    event Action<ChecklistItemRenamedDto>? OnChecklistItemRenamed;

    event Action<ChecklistItemStatusUpdatedDto>? OnChecklistItemStatusUpdated;

    event Action<AttachmentAddedDto>? OnAttachmentAdded;

    event Action<AttachmentDeletedDto>? OnAttachmentDeleted;

    Task StartConnectionAsync(
        string backendUrl,
        Guid boardId,
        CancellationToken cancellationToken = default);

    Task StopConnectionAsync(
        Guid boardId,
        CancellationToken cancellationToken = default);
}

namespace WorkBoard.Services.Abstraction.DTOs.Checklists;

public record ChecklistItemStatusUpdatedDto(
    Guid CardId,
    Guid ChecklistId,
    Guid ItemId,
    bool IsDone);

namespace WorkBoard.Services.Abstraction.DTOs;

public record ChecklistItemStatusUpdatedDto(
    Guid CardId,
    Guid ChecklistId,
    Guid ItemId,
    bool IsDone);

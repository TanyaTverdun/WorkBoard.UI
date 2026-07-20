namespace WorkBoard.Services.Abstraction.DTOs;

public record ChecklistItemStatusUpdatedDto(
    Guid ChecklistId,
    Guid ItemId,
    bool IsDone);

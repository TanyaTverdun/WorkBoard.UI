namespace WorkBoard.Services.Abstraction.DTOs.Checklists;

public record ChecklistItemDeletedDto(
    Guid CardId,
    Guid ChecklistId,
    ChecklistItemDto Item);

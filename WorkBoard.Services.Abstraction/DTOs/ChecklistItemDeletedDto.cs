namespace WorkBoard.Services.Abstraction.DTOs;

public record ChecklistItemDeletedDto(
    Guid CardId,
    Guid ChecklistId,
    ChecklistItemDto Item);

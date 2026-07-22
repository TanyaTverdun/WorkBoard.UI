namespace WorkBoard.Services.Abstraction.DTOs.Checklists;

public record ChecklistItemAddedDto(
    Guid CardId,
    Guid ChecklistId, 
    ChecklistItemDto Item);

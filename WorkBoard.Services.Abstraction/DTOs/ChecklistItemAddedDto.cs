namespace WorkBoard.Services.Abstraction.DTOs;

public record ChecklistItemAddedDto(
    Guid CardId,
    Guid ChecklistId, 
    ChecklistItemDto Item);

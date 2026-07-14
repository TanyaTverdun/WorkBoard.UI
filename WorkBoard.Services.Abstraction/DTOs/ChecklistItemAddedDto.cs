namespace WorkBoard.Services.Abstraction.DTOs;

public record ChecklistItemAddedDto(
    Guid ChecklistId, 
    ChecklistItemDto Item);

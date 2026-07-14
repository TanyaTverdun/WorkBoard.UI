namespace WorkBoard.Services.Abstraction.DTOs;

public record ChecklistCreatedDto(
    Guid CardId, 
    ChecklistDto Checklist);

namespace WorkBoard.Services.Abstraction.DTOs.Checklists;

public record ChecklistCreatedDto(
    Guid CardId, 
    ChecklistDto Checklist);

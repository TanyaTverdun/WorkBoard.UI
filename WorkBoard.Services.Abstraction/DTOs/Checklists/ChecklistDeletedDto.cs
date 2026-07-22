namespace WorkBoard.Services.Abstraction.DTOs.Checklists;

public record ChecklistDeletedDto(
    Guid CardId, 
    Guid ChecklistId);

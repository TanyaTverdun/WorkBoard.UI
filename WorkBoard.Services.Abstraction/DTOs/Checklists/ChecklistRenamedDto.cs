namespace WorkBoard.Services.Abstraction.DTOs.Checklists;

public record ChecklistRenamedDto(
    Guid ChecklistId, 
    string NewName);

namespace WorkBoard.Services.Abstraction.DTOs.Checklists;

public record ChecklistItemRenamedDto(
    Guid ChecklistId, 
    Guid ItemId, 
    string NewTitle);

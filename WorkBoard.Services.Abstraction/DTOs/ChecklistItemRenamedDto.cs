namespace WorkBoard.Services.Abstraction.DTOs;

public record ChecklistItemRenamedDto(
    Guid ChecklistId, 
    Guid ItemId, 
    string NewTitle);

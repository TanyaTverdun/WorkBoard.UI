namespace WorkBoard.Services.Abstraction.DTOs;

public record ChecklistRenamedDto(
    Guid ChecklistId, 
    string NewName);

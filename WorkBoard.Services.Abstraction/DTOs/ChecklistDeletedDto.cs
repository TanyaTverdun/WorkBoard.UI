namespace WorkBoard.Services.Abstraction.DTOs;

public record ChecklistDeletedDto(
    Guid CardId, 
    Guid ChecklistId);

namespace WorkBoard.Services.Abstraction.DTOs;

public record ChecklistItemDeletedDto(
    Guid ChecklistId, 
    Guid ItemId);

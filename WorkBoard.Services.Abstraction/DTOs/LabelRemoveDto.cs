namespace WorkBoard.Services.Abstraction.DTOs;

public record LabelRemoveDto(
    Guid CardId, 
    Guid LabelId);

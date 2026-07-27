namespace WorkBoard.Services.Abstraction.DTOs.Labels;

public record LabelRemoveDto(
    Guid CardId, 
    Guid LabelId);

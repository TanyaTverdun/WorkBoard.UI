namespace WorkBoard.Services.Abstraction.DTOs.Labels;

public record LabelAddDto(
    Guid CardId, 
    LabelDto Label);

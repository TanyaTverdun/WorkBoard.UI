namespace WorkBoard.Services.Abstraction.DTOs;

public record LabelAddDto(
    Guid CardId, 
    LabelDto Label);

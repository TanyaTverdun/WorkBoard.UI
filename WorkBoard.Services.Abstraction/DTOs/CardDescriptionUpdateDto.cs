namespace WorkBoard.Services.Abstraction.DTOs;

public record CardDescriptionUpdateDto(
    Guid CardId,
    string Description);

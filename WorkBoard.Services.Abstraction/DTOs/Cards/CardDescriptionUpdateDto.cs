namespace WorkBoard.Services.Abstraction.DTOs.Cards;

public record CardDescriptionUpdateDto(
    Guid CardId,
    string Description);

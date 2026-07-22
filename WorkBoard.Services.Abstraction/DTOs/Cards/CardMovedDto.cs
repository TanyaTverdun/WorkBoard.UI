namespace WorkBoard.Services.Abstraction.DTOs.Cards;

public record CardMovedDto(
    Guid CardId,
    Guid NewSectionId,
    string NewSectionName,
    double NewPosition);

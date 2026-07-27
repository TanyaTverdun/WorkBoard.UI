namespace WorkBoard.Services.Abstraction.Requests.Cards;

public record MoveCardRequest(
    Guid NewSectionId,
    double NewPosition);

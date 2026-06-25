namespace WorkBoard.Services.Abstraction.Requestsж;

public record MoveCardRequest(
    Guid NewSectionId,
    double NewPosition);

namespace WorkBoard.Services.Abstraction.Requests.Cards;

public record CreateCardRequest(
    string Title,
    double Position);

namespace WorkBoard.Services.Abstraction.Requests;

public record CreateCardRequest(
    string Title,
    double Position);

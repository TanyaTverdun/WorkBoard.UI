namespace WorkBoard.Services.Abstraction.Requests;

public record UpdateLabelRequest(
    string Name,
    string Color);

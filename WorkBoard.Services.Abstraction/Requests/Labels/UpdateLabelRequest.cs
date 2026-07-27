namespace WorkBoard.Services.Abstraction.Requests.Labels;

public record UpdateLabelRequest(
    string Name,
    string Color);

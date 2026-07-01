namespace WorkBoard.Services.Abstraction.DTOs;

public record UpdateLabelRequest(
    string Name,
    string Color);

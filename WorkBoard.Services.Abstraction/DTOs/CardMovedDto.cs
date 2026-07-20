namespace WorkBoard.Services.Abstraction.DTOs;

public record CardMovedDto(
    Guid CardId,
    Guid NewSectionId,
    string NewSectionName,
    double NewPosition);

namespace WorkBoard.Services.Abstraction.DTOs;

public record CardRenameDto(
    Guid CardId, 
    string NewTitle);

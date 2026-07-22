namespace WorkBoard.Services.Abstraction.DTOs.Cards;

public record CardRenameDto(
    Guid CardId, 
    string NewTitle);

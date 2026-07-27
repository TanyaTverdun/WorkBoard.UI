namespace WorkBoard.Services.Abstraction.DTOs.Cards;

public record AssigneeRemoveDto(
    Guid CardId,
    Guid UserId);

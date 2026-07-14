namespace WorkBoard.Services.Abstraction.DTOs;

public record AssigneeRemoveDto(
    Guid CardId,
    Guid UserId);

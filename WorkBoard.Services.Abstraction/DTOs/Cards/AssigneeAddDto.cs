namespace WorkBoard.Services.Abstraction.DTOs.Cards;

public record AssigneeAddDto(
    Guid CardId,
    CardAssigneeDto Assignee);

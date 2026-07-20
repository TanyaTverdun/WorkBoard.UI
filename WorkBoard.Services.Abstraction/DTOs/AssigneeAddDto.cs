namespace WorkBoard.Services.Abstraction.DTOs;

public record AssigneeAddDto(
    Guid CardId,
    CardAssigneeDto Assignee);

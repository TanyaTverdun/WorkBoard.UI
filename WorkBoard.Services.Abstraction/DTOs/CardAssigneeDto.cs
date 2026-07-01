namespace WorkBoard.Services.Abstraction.DTOs;

public record CardAssigneeDto(
    Guid UserId,
    string FullName,
    string Email,
    string? AvatarUrl,
    string Initials
);

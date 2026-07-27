namespace WorkBoard.Services.Abstraction.DTOs.Cards;

public record CardAssigneeDto(
    Guid UserId,
    string FullName,
    string Email,
    string? AvatarUrl,
    string? AvatarColor,
    string Initials
);

using WorkBoard.Domain.Enums;

namespace WorkBoard.Services.Abstraction.DTOs.Workspaces;

public record WorkspaceMemberAddedDto(
    Guid UserId,
    string Name,
    string Email,
    WorkspaceRole Role,
    string? AvatarUrl,
    string? AvatarColor,
    string? Initials
);

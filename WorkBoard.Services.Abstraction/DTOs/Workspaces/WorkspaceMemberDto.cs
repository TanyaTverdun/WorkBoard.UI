using WorkBoard.Domain.Enums;

namespace WorkBoard.Services.Abstraction.DTOs.Workspaces;

public class WorkspaceMemberDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public WorkspaceRole Role { get; set; }
    public bool IsCurrentUser { get; set; }
    public string? AvatarUrl { get; set; }
    public string? AvatarColor { get; set; }
    public string? Initials { get; set; }
}

using WorkBoard.Domain.Enums;

namespace WorkBoard.Services.Abstraction.DTOs.Workspaces;

public record WorkspaceMemberRoleUpdatedDto(
    Guid UserId, 
    WorkspaceRole NewRole);

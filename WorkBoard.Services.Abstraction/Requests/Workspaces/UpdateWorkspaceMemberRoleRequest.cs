using WorkBoard.Domain.Enums;

namespace WorkBoard.Services.Abstraction.Requests.Workspaces;

public class UpdateWorkspaceMemberRoleRequest
{
    public WorkspaceRole NewRole { get; set; }
}

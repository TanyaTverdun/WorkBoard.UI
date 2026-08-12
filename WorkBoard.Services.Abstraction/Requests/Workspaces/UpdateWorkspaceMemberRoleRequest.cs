using WorkBoard.Domain.Enums;

namespace WorkBoard.Services.Abstraction.Requests.Workspaces;

public class UpdateWorkspaceMemberRoleRequest
{
    public int NewRole { get; set; }
}

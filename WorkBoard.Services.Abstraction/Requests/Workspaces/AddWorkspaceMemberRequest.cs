using WorkBoard.Domain.Enums;

namespace WorkBoard.Services.Abstraction.Requests.Workspaces;

public class AddWorkspaceMemberRequest
{
    public Guid UserId { get; set; }
    public WorkspaceRole Role { get; set; }
}

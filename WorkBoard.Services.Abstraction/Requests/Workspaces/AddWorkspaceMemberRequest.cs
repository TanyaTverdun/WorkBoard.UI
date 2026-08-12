using WorkBoard.Domain.Enums;

namespace WorkBoard.Services.Abstraction.Requests.Workspaces;

public class AddWorkspaceMemberRequest
{
    public Guid UserId { get; set; }
    public int Role { get; set; }
}

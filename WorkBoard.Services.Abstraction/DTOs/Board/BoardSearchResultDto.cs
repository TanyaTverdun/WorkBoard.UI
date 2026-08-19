using WorkBoard.Domain.Enums;

namespace WorkBoard.Services.Abstraction.DTOs.Board;

public class BoardSearchResultDto
{
    public Guid BoardId { get; set; }
    public string BoardName { get; set; } = string.Empty;
    public Guid WorkspaceId { get; set; }
    public string WorkspaceName { get; set; } = string.Empty;
    public WorkspaceRole Role { get; set; }
    public SubscriptionTier SubscriptionTier { get; set; }
}

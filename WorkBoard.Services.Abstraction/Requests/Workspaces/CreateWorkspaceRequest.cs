using System.ComponentModel.DataAnnotations;

namespace WorkBoard.Services.Abstraction.Requests.Workspaces;

public class CreateWorkspaceRequest
{
    [Required(ErrorMessage = "Workspace name is required")]
    [StringLength(50, ErrorMessage = "Workspace name cannot exceed 50 characters")]
    public string Name { get; set; } = string.Empty;
}

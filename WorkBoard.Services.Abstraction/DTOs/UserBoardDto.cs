using WorkBoard.Domain.Enums;

namespace WorkBoard.Services.Abstraction.DTOs;

public class BoardDto
{
    public Guid Id { get; set; }
    public Guid WorkspaceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public BoardRole UserRole { get; set; }
}

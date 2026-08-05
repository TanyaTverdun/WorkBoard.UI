using WorkBoard.Domain.Enums;

namespace WorkBoard.Services.Abstraction.DTOs.Board;

public class BoardArchivationDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string WorkspaceName { get; set; }
    public BoardArchiveStatus ArchiveStatus { get; set; }
}

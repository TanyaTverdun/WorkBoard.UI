namespace WorkBoard.Services.Abstraction.DTOs.Board;

public class BoardArchiveStatusUpdatedDto
{
    public Guid BoardId { get; set; }
    public bool IsArchived { get; set; }
}

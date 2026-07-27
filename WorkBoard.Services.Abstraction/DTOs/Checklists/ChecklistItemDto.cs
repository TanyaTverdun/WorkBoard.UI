namespace WorkBoard.Services.Abstraction.DTOs.Checklists;

public class ChecklistItemDto
{
    public Guid ChecklistId { get; set; }
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsDone { get; set; }
}

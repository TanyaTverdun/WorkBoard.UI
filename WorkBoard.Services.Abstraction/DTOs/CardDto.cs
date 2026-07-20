namespace WorkBoard.Services.Abstraction.DTOs;

public class CardDto
{
    public Guid Id { get; set; }
    public Guid SectionId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public double Position { get; set; }
    public int CommentsCount { get; set; }
    public int AttachmentsCount { get; set; }
    public int ChecklistTotalItems { get; set; }
    public int ChecklistDoneItems { get; set; }
    public List<LabelDto> Labels { get; set; } = new();
    public List<CardAssigneeDto> Assignees { get; set; } = new();
}

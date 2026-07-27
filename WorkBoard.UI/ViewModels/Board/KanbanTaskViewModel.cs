using WorkBoard.Services.Abstraction.DTOs.Cards;
using WorkBoard.Services.Abstraction.DTOs.Labels;

namespace WorkBoard.UI.ViewModels.Board;

public class KanbanTaskViewModel
{
    public Guid Id { get; init; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; }
    public double Position { get; set; }
    public DateTime? DueDate { get; set; }
    public Guid BoardId { get; set; }
    public Guid SectionId { get; set; }
    public int CommentsCount { get; set; }
    public int AttachmentsCount { get; set; }
    public int ChecklistTotalItems { get; set; }
    public int ChecklistDoneItems { get; set; }
    public List<LabelDto> Labels { get; set; } = new();
    public List<CardAssigneeDto> Assignees { get; set; } = new();
}

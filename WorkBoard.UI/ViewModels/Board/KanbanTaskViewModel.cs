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

    public KanbanTaskViewModel(
        Guid id,
        string name,
        string status,
        double position,
        Guid boardId,
        Guid sectionId,
        string? description,
        DateTime? dueDate)
    {
        Id = id;
        Name = name;
        Status = status;
        Position = position;
        BoardId = boardId;
        SectionId = sectionId;
        Description = description;
        DueDate = dueDate;
    }
}

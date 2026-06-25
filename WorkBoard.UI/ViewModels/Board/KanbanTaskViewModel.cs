namespace WorkBoard.UI.ViewModels.Board;

public class KanbanTaskViewModel
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Status { get; set; }
    public double Position { get; set; }

    public KanbanTaskViewModel(
        Guid id,
        string name,
        string status,
        double position)
    {
        Id = id;
        Name = name;
        Status = status;
        Position = position;
    }
}

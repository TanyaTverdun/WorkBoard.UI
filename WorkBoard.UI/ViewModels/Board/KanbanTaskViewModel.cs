namespace WorkBoard.UI.ViewModels.Board;

public class KanbanTaskViewModel
{
    public string Name { get; init; }
    public string Status { get; set; }

    public KanbanTaskViewModel(
        string name,
        string status)
    {
        Name = name;
        Status = status;
    }
}

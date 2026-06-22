namespace WorkBoard.UI.ViewModels.Board;

public class KanbanSectionViewModel
{
    public Guid Id { get; init; }
    public string Name { get; set; }
    public bool NewTaskOpen { get; set; }
    public string NewTaskName { get; set; }
    public bool IsConfirmingDelete { get; set; }
    public bool IsRenaming { get; set; }
    public string? EditName { get; set; }
    public double Position { get; set; }
    public bool IsPositionChanged { get; set; }

    private bool _menuOpen;
    public bool MenuOpen
    {
        get => _menuOpen;
        set
        {
            _menuOpen = value;
            if (!value) IsConfirmingDelete = false;
        }
    }

    public KanbanSectionViewModel(
        Guid id,
        string name,
        bool newTaskOpen,
        string newTaskName,
        double position = 0.0)
    {
        Id = id;
        Name = name;
        NewTaskOpen = newTaskOpen;
        NewTaskName = newTaskName;
        Position = position;
        IsConfirmingDelete = false;
        IsRenaming = false;
    }
}

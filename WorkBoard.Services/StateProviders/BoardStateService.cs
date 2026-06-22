namespace WorkBoard.Services.StateProviders;

public class BoardStateService
{
    private string _currentBoardName = "Board";

    public event Action? OnBoardNameChanged;

    public string CurrentBoardName
    {
        get => _currentBoardName;
        private set
        {
            if (_currentBoardName != value)
            {
                _currentBoardName = value;
                OnBoardNameChanged?.Invoke();
            }
        }
    }

    public void SetBoardName(string name)
    {
        CurrentBoardName = name;
    }
}

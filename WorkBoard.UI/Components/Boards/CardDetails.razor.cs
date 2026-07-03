using Microsoft.AspNetCore.Components;
using MudBlazor;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Requests;
using WorkBoard.Services.Abstraction.Services;
using WorkBoard.UI.ViewModels.Board;

namespace WorkBoard.UI.Components.Boards;

public partial class CardDetails
{
    [CascadingParameter] 
    IMudDialogInstance MudDialog { get; set; } = default!;

    [Parameter] 
    public KanbanTaskViewModel Card { get; set; } = default!;

    [Inject] 
    private ICardService CardService { get; set; } = default!;

    [Inject]
    private IChecklistService ChecklistService { get; set; } = default!;

    private const string DefaultChecklistTitle = "New Checklist";

    private bool _isPendingDeleteCard = false;

    private bool _isEditingTitle = false;
    private string _editedTitle = string.Empty;
    private bool _isSavingTitle = false;

    private bool _isEditingDescription = false;
    private string _editedDescription = string.Empty;
    private bool _isSavingDescription = false;

    private List<CardAssigneeDto> _assignees = new();
    private List<UserSearchDto> _assignableUsers = new();

    private bool _isAssigneePopoverOpen = false;
    private string _assigneeSearchText = string.Empty;

    private ChecklistDto? _currentChecklist;
    private bool _hasChecklist = false;
    private string _checklistTitle = DefaultChecklistTitle;
    private bool _isHoveringChecklistTitle = false;
    private bool _isEditingChecklistTitle = false;
    private string _editedChecklistTitle = string.Empty;
    private bool _isPendingDeleteChecklist = false;

    private IEnumerable<UserSearchDto> FilteredAssignableUsers =>
        string.IsNullOrWhiteSpace(_assigneeSearchText)
            ? _assignableUsers
            : _assignableUsers.Where(u =>
                u.FullName.Contains(_assigneeSearchText, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains(_assigneeSearchText, StringComparison.OrdinalIgnoreCase));

    protected override async Task OnInitializedAsync()
    {
        _editedTitle = Card.Name;
        _editedDescription = Card.Description ?? string.Empty;

        await Task.WhenAll(
            LoadAssigneesDataAsync(),
            LoadChecklistAsync()
        );
    }

    private async Task LoadChecklistAsync()
    {
        try
        {
            _currentChecklist = await ChecklistService.GetChecklistByCardAsync(Card.Id);

            if (_currentChecklist != null)
            {
                _hasChecklist = true;
                _checklistTitle = _currentChecklist.Name;
            }
            else
            {
                _hasChecklist = false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading checklist: {ex.Message}");
        }
    }

    private void StartAddingChecklist()
    {
        _currentChecklist = null;
        _hasChecklist = true;

        _checklistTitle = DefaultChecklistTitle;
        _editedChecklistTitle = _checklistTitle;
        _isEditingChecklistTitle = true;
        _isPendingDeleteChecklist = false;
    }

    private void EnableChecklistTitleEdit()
    {
        _editedChecklistTitle = _checklistTitle;
        _isEditingChecklistTitle = true;
        _isPendingDeleteChecklist = false;
    }

    private async Task SaveChecklistTitle()
    {

        if (string.IsNullOrWhiteSpace(_editedChecklistTitle))
        {
            if (_currentChecklist == null) 
            {
                _hasChecklist = false;
            }

            _isEditingChecklistTitle = false;
            return;
        }

        var trimmedTitle = _editedChecklistTitle.Trim();

        if (_currentChecklist != null && trimmedTitle == _checklistTitle)
        {
            _isEditingChecklistTitle = false;
            return;
        }

        try
        {
            if (_currentChecklist != null)
            {
                var request = new UpdateChecklistRequest 
                { 
                    Name = trimmedTitle 
                };

                await ChecklistService.UpdateChecklistAsync(
                    _currentChecklist.ChecklistId, 
                    request);

                _checklistTitle = trimmedTitle;
            }
            else
            {
                var request = new CreateChecklistRequest 
                { 
                    Name = trimmedTitle 
                };

                _currentChecklist = await ChecklistService.CreateChecklistAsync(
                    Card.Id, 
                    request);

                _checklistTitle = _currentChecklist.Name;
            }
        }
        finally
        {
            _isEditingChecklistTitle = false;
            StateHasChanged();
        }
    }

    private void CancelChecklistTitleEdit()
    {
        _isEditingChecklistTitle = false;
    }

    private async Task ConfirmDeleteChecklist()
    {
        if (_currentChecklist != null)
        {
            try
            {
                await ChecklistService.DeleteChecklistAsync(
                    _currentChecklist.ChecklistId);
                _currentChecklist = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting checklist: {ex.Message}");
                _isPendingDeleteChecklist = false;
                StateHasChanged();
                return;
            }
        }

        _checklist.Clear();
        _hasChecklist = false;
        _checklistTitle = DefaultChecklistTitle;
        _isPendingDeleteChecklist = false;
        _isHoveringChecklistTitle = false;

        StateHasChanged();
    }

    private async Task LoadAssigneesDataAsync()
    {
        try
        {
            var assigneesTask = CardService.GetCardAssigneesAsync(Card.Id);
            var assignableTask = CardService.GetAssignableUsersAsync(Card.Id);

            await Task.WhenAll(assigneesTask, assignableTask);

            _assignees = assigneesTask.Result.ToList();
            _assignableUsers = assignableTask.Result.ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading assignees data: {ex.Message}");
        }
        finally
        {
            StateHasChanged();
        }
    }

    private async Task AddAssigneeAsync(UserSearchDto user)
    {
        try
        {
            var request = new AddCardAssigneeRequest(user.UserId);
            await CardService.AddCardAssigneeAsync(Card.Id, request);

            await LoadAssigneesDataAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding assignee: {ex.Message}");
        }
    }

    private async Task RemoveAssigneeAsync(CardAssigneeDto assignee)
    {
        try
        {
            await CardService.RemoveAssigneeAsync(Card.Id, assignee.UserId);
            await LoadAssigneesDataAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing assignee: {ex.Message}");
        }
    }

    private async Task ConfirmDeleteCardAsync()
    {
        try
        {
            await CardService.DeleteCardAsync(Card.BoardId, Card.Id);

            MudDialog.Close(DialogResult.Ok(true));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting card: {ex.Message}");
        }
    }

    private async Task SaveTitleAsync()
    {
        _isSavingTitle = true;

        if (string.IsNullOrWhiteSpace(_editedTitle))
        {
            _isSavingTitle = false;
            return;
        }

        var trimmedTitle = _editedTitle.Trim();

        if (trimmedTitle == Card.Name)
        {
            _isEditingTitle = false;
            _isSavingTitle = false;

            return;
        }

        try
        {
            await CardService.UpdateCardTitleAsync(
                Card.BoardId,
                Card.Id,
                new UpdateCardTitleRequest(trimmedTitle));

            Card.Name = trimmedTitle;
            _isEditingTitle = false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating title: {ex.Message}");
        }
        finally
        {
            StateHasChanged();
            _isSavingTitle = false;
        }
    }

    private async Task OnTitleBlurAsync()
    {
        await Task.Delay(200);

        if (_isSavingTitle)
        {
            return;
        }

        if (!_isEditingTitle)
        {
            return;
        }

        DiscardTitleChanges();
        StateHasChanged();
    }

    private void DiscardTitleChanges()
    {
        _editedTitle = Card.Name;
        _isEditingTitle = false;
    }

    private async Task SaveDescriptionAsync()
    {
        _isSavingDescription = true;

        var trimmedDesc = _editedDescription?.Trim() ?? string.Empty;

        if (trimmedDesc == Card.Description)
        {
            _isEditingDescription = false;
            _isSavingDescription = false;
            return;
        }

        try
        {
            await CardService.UpdateCardDescriptionAsync(
                Card.BoardId,
                Card.Id,
                new UpdateCardDescriptionRequest(trimmedDesc));

            Card.Description = trimmedDesc;
            _isEditingDescription = false;        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating description: {ex.Message}");
        }
        finally
        {
            StateHasChanged();
            _isSavingDescription = false;
        }
    }

    private void DiscardDescriptionChanges()
    {
        _editedDescription = Card.Description ?? string.Empty;
        _isEditingDescription = false;
    }

    private async Task OnDescriptionBlurAsync()
    {
        await Task.Delay(200);

        if (_isSavingDescription)
        {
            return;
        }

        if (!_isEditingDescription)
        {
            return;
        }

        DiscardDescriptionChanges();
        StateHasChanged();
    }



    /// /////////МОКИ///////////////////////////////////////////////////////////////////////////////
    private DateTime? _dueDate = null;
    private string _newComment = string.Empty;

    private List<CardChecklistItemMock> _checklist = new()
    {
    };

    private List<CardAttachmentMock> _attachments = new()
    {
        new CardAttachmentMock("signalr-architecture.pdf", "1.2 MB", Icons.Material.Filled.Description, Color.Error),
        new CardAttachmentMock("hub-diagram.png", "340 KB", Icons.Material.Filled.Image, Color.Info),
        new CardAttachmentMock("signalr-architecture.pdf", "1.2 MB", Icons.Material.Filled.Description, Color.Error),
        new CardAttachmentMock("signalr-architecture.pdf", "1.2 MB", Icons.Material.Filled.Description, Color.Error),
        new CardAttachmentMock("signalr-architecture.pdf", "1.2 MB", Icons.Material.Filled.Description, Color.Error),
        new CardAttachmentMock("signalr-architecture.pdf", "1.2 MB", Icons.Material.Filled.Description, Color.Error),
        new CardAttachmentMock("signalr-architecture.pdf", "1.2 MB", Icons.Material.Filled.Description, Color.Error)
    };

    private List<CardCommentMock> _comments = new()
    {
        new CardCommentMock(
            "Mikhail Ivanov", "MI", 
            Color.Primary, new DateTime(2026, 5, 21, 13, 30, 0), 
            "I have drafted the hub architecture. Sharing the diagram now."),
        new CardCommentMock(
            "Sarah Chen", "SC", 
            Color.Success, new DateTime(2026, 5, 21, 14, 15, 0), 
            "Looks good! I will start on connection management tomorrow.")
    };

    public class CardChecklistItemMock
    {
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
        public CardChecklistItemMock(
            string title, 
            bool isCompleted) 
        { 
            Title = title; 
            IsCompleted = isCompleted; 
        }
    }
    public record CardAttachmentMock(
        string FileName, 
        string FileSize, 
        string Icon, 
        Color IconColor);
    public record CardCommentMock(
        string AuthorName, 
        string Initials, 
        Color AvatarColor, 
        DateTime Date, 
        string Text);
  
    private int CompletedChecklistItems => _checklist.Count(x => x.IsCompleted);
    private int TotalChecklistItems => _checklist.Count;
    private double ChecklistProgress => TotalChecklistItems == 0 ? 0 
        : Math.Round((double)CompletedChecklistItems / TotalChecklistItems * 100);

    private void Close() => MudDialog.Cancel();

    private bool _isAddingChecklistItem = false;
    private string _newChecklistItemTitle = string.Empty;

    private void ShowAddChecklistItemForm()
    {
        _isAddingChecklistItem = true;
        _newChecklistItemTitle = string.Empty;
    }

    private void CancelAddChecklistItem()
    {
        _isAddingChecklistItem = false;
        _newChecklistItemTitle = string.Empty;
    }

    private void AddChecklistItem()
    {
        if (string.IsNullOrWhiteSpace(_newChecklistItemTitle)) return;

        _checklist.Add(new CardChecklistItemMock(_newChecklistItemTitle.Trim(), false));

        _newChecklistItemTitle = string.Empty;

        StateHasChanged();
    }
}

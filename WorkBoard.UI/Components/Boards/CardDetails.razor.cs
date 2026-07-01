using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
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
    private ILabelService LabelService { get; set; } = default!;

    private bool _isEditingTitle = false;
    private string _editedTitle = string.Empty;
    private bool _isSavingTitle = false;


    private bool _isEditingDescription = false;
    private string _editedDescription = string.Empty;
    private bool _isSavingDescription = false;

    private List<LabelDto> _allAvailableLabels = new();
    private List<LabelDto> _labels = new();
    private MudColor _newLabelColor = new("#4a4388ff");

    private bool _isLabelPopoverOpen = false;
    private string _labelSearchText = string.Empty;
    private bool _isCreatingNewLabel = false;
    private string _newLabelName = string.Empty;
    private Guid? _pendingDeleteLabelId = null;

    protected override async Task OnInitializedAsync()
    {
        base.OnInitialized();
        _editedTitle = Card.Name;
        _editedDescription = Card.Description ?? string.Empty;
        await LoadLabelsAsync();

        try
        {
            var cardLabels = await LabelService.GetLabelsByCardAsync(Card.Id);
            _labels = cardLabels.ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading card labels: {ex.Message}");
        }
    }

    private async Task LoadLabelsAsync()
    {
        try
        {
            var labels = await LabelService.GetLabelsByBoardAsync(Card.BoardId);
            _allAvailableLabels = labels.ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading labels: {ex.Message}");
        }
    }

    private async Task ToggleLabelAsync(LabelDto label)
    {
        var existingLabel = _labels.FirstOrDefault(x => x.Id == label.Id);
        if (existingLabel != null)
        {
            try
            {
                await LabelService.RemoveLabelFromCardAsync(Card.Id, label.Id);
                _labels.Remove(existingLabel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing label: {ex.Message}");
            }
        }
        else
        {
            try
            {
                await LabelService.AddLabelToCardAsync(Card.Id, label.Id);
                _labels.Add(label);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error attaching label: {ex.Message}");
            }
        }
        StateHasChanged();
    }

    private IEnumerable<LabelDto> FilteredLabels =>
        string.IsNullOrWhiteSpace(_labelSearchText)
            ? _allAvailableLabels
            : _allAvailableLabels.Where(l => l.Name.Contains(
                _labelSearchText, StringComparison.OrdinalIgnoreCase));

    private void InitiateDeleteLabel(Guid labelId)
    {
        _pendingDeleteLabelId = labelId;
    }

    private void CancelDeleteLabel()
    {
        _pendingDeleteLabelId = null;
    }

    private async Task ConfirmDeleteLabelAsync(LabelDto label)
    {
        try
        {
            await LabelService.DeleteLabelAsync(label.Id);

            _allAvailableLabels.RemoveAll(l => l.Id == label.Id);
            _labels.RemoveAll(l => l.Id == label.Id);

            _pendingDeleteLabelId = null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting label: {ex.Message}");
        }
        finally
        {
            StateHasChanged();
        }
    }

    private Guid? _editingLabelId = null;
    private string _editingLabelName = string.Empty;
    private MudColor _editingLabelColor = new("#4a4388ff");

    private void EditLabel(LabelDto label)
    {
        _editingLabelId = label.Id;
        _editingLabelName = label.Name;
        _editingLabelColor = new MudColor(label.Color ?? "#4a4388ff");
    }

    private void CancelEditLabel()
    {
        _editingLabelId = null;
    }

    private async Task SaveEditLabelAsync()
    {
        if (string.IsNullOrWhiteSpace(_editingLabelName) || _editingLabelId == null) return;

        try
        {
            var request = new UpdateLabelRequest(_editingLabelName, _editingLabelColor.Value);

            await LabelService.UpdateLabelAsync(_editingLabelId.Value, request);

            var label = _allAvailableLabels.FirstOrDefault(l => l.Id == _editingLabelId);
            if (label != null)
            {
                label.Name = _editingLabelName;
                label.Color = _editingLabelColor.Value;
            }

            _editingLabelId = null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating label: {ex.Message}");
        }
    }

    private void ShowCreateLabelForm()
    {
        _isCreatingNewLabel = true;
        _newLabelName = string.Empty;
        _newLabelColor = new("#4a4388ff");
    }

    private void HideCreateLabelForm()
    {
        _isCreatingNewLabel = false;
        _newLabelName = string.Empty;
    }

    private async Task CreateNewLabelAsync()
    {
        if (string.IsNullOrWhiteSpace(_newLabelName)) return;

        var trimmedName = _newLabelName.Trim();

        if (_allAvailableLabels.Any(
            l => l.Name.Equals(trimmedName, StringComparison.OrdinalIgnoreCase)))
        {
            HideCreateLabelForm();
            return;
        }

        try
        {
            var request = new CreateLabelRequest
            {
                Name = trimmedName,
                Color = _newLabelColor.Value
            };

            var newLabel = await LabelService.CreateLabelAsync(
                Card.Id, 
                request);

            _allAvailableLabels.Add(newLabel);
            _labels.Add(newLabel);

            HideCreateLabelForm();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating label: {ex.Message}");
        }
        finally
        {
            StateHasChanged();
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
        new CardChecklistItemMock("Design hub architecture", true),
        new CardChecklistItemMock("Implement connection management", true),
        new CardChecklistItemMock("Write delta broadcast logic", false),
        new CardChecklistItemMock("Integration tests", false)
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

    public record CardAssigneeMock(
        string FullName, 
        string Initials, 
        Color AvatarColor, 
        string Email, 
        string Role);
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

    private List<CardAssigneeMock> _assignees = new()
    {
        new CardAssigneeMock(
            "Mikhail Ivanov", "MI", 
            Color.Secondary, 
            "mikhail@workboard.com", "Admin"),
        new CardAssigneeMock(
            "Sarah Chen", "SC", 
            Color.Success, 
            "sarah@workboard.com", "Member")
    };

    private List<CardAssigneeMock> _allBoardMembers = new()
    {
        new CardAssigneeMock("Alexandra Petrova", "AP", Color.Primary, "alexandra@workboard.com", "Owner"),
        new CardAssigneeMock("Mikhail Ivanov", "MI", Color.Secondary, "mikhail@workboard.com", "Admin"),
        new CardAssigneeMock("Sarah Chen", "SC", Color.Success, "sarah@workboard.com", "Member"),
        new CardAssigneeMock("David Nakamura", "DN", Color.Warning, "david@workboard.com", "Member"),
        new CardAssigneeMock("Elena Novak", "EN", Color.Error, "elena@workboard.com", "Observer")
    };

  
    private int CompletedChecklistItems => _checklist.Count(x => x.IsCompleted);
    private int TotalChecklistItems => _checklist.Count;
    private double ChecklistProgress => TotalChecklistItems == 0 ? 0 
        : Math.Round((double)CompletedChecklistItems / TotalChecklistItems * 100);

    private void Close() => MudDialog.Cancel();

    

    
    

    
    private bool _isAssigneePopoverOpen = false;
    private string _assigneeSearchText = string.Empty;

    private IEnumerable<CardAssigneeMock> FilteredAssignees => _allBoardMembers.ToList();

    private void ToggleAssignee(CardAssigneeMock member)
    {
        var existing = _assignees.FirstOrDefault(m => m.Email == member.Email);
        if (existing != null)
        {
            _assignees.Remove(existing);
        }
        else
        {
            _assignees.Add(member);
        }
        StateHasChanged();
    }

    private void RemoveAssignee(CardAssigneeMock assignee)
    {
        _assignees.Remove(assignee);
        StateHasChanged();
    }

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

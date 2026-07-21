using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Hubs;
using WorkBoard.Services.Abstraction.Requests;
using WorkBoard.Services.Abstraction.Services;
using WorkBoard.UI.ViewModels.Board;

namespace WorkBoard.UI.Components.Card;

public partial class CardDetails: ComponentBase, IDisposable
{
    private const string ImportCommand = "import";

    private const string JsModulePath = "./Components/Boards/CardDetails.razor.js";

    [CascadingParameter] 
    IMudDialogInstance MudDialog { get; set; } = default!;

    [Parameter] 
    public KanbanTaskViewModel Card { get; set; } = default!;

    [Parameter]
    public bool IsObserver { get; set; }

    [Parameter]
    public Guid CurrentUserId { get; set; }

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    [Inject] 
    private ICardService CardService { get; set; } = default!;

    [Inject]
    private IAttachmentService AttachmentService { get; set; } = default!;

    [Inject]
    private IBoardHubService BoardHubService { get; set; } = default!;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    private ElementReference _mainContentScroll;
    private IJSObjectReference? _jsModule;

    private bool _isPendingDeleteCard = false;
    private bool _isDeletingLocally = false;

    private DateTime? _dueDate;

    private bool _isEditingTitle = false;
    private string _editedTitle = string.Empty;
    private bool _isSavingTitle = false;

    private bool _isEditingDescription = false;
    private string _editedDescription = string.Empty;
    private bool _isSavingDescription = false;

    private List<CardAssigneeDto> _assignees = new();
    private List<UserSearchDto> _assignableUsers = new();

    private int _commentsCount = 0;

    private List<AttachmentDto> _attachments = new();

    private List<ActivityLogDto> _activityLogs = new();

    private ChecklistDto? _checklist;
    private List<LabelDto> _cardLabels = new();
    private List<CommentDto> _comments = new();

    private bool _isPickerLoaded = false;
    private MudDatePicker _datePicker;

    protected override async Task OnInitializedAsync()
    {
        BoardHubService.OnCardDueDateUpdated += HandleCardDueDateUpdated;
        BoardHubService.OnCardMoved += HandleCardMoved;
        BoardHubService.OnCardDescriptionUpdated += HandleDescriptionUpdated;
        BoardHubService.OnCardRenamed += HandleCardRenamed;
        BoardHubService.OnCardDeleted += HandleCardDeleted;
        BoardHubService.OnActivityLogAdded += HandleActivityLogAdded;

        var sw = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var cardDetails = await CardService.GetCardDetailsAsync(
                Card.SectionId,
                Card.Id);

            _editedTitle = cardDetails.Title;
            _editedDescription = cardDetails.Description ?? string.Empty;
            _dueDate = cardDetails.DueDate?.Date;
            _assignees = cardDetails.Assignees.ToList();
            _cardLabels = cardDetails.Labels.ToList();
            _checklist = cardDetails.Checklist;
            _attachments = cardDetails.Attachments.ToList();
            _comments = cardDetails.Comments.ToList();
            _activityLogs = cardDetails.ActivityLogs.ToList();
            _commentsCount = _comments.Count;

            var assignableUsers = await CardService.GetAssignableUsersAsync(
                Card.Id);

            _assignableUsers = assignableUsers.ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading card details: {ex.Message}");
            Snackbar.Add("Failed to load card details", Severity.Error);
        }

        sw.Stop();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                    ImportCommand,
                    JsModulePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading JS module: {ex.Message}");
            }
        }
    }

    private async Task LoadAndOpenPicker()
    {
        if (IsObserver)
        {
            return;
        }

        _isPickerLoaded = true;
        StateHasChanged();

        await Task.Delay(50);

        _datePicker?.OpenAsync();
    }

    private async Task OnDueDateChangedAsync(DateTime? newDate)
    {
        if (IsObserver)
        {
            return;
        }

        if (_dueDate?.Date == newDate?.Date)
        {
            return;
        }

        _dueDate = newDate;

        try
        {
            var request = new UpdateCardDueDateRequest 
            { 
                DueDate = newDate 
            };

            await CardService.UpdateCardDueDateAsync(
                Card.BoardId,
                Card.Id,
                request);

             Card.DueDate = newDate; 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating due date: {ex.Message}");
            Snackbar.Add("Failed to update due date.", Severity.Error);
        }
    }

    private async Task UpdateCommentsCount(int count)
    {
        _commentsCount = count;
        StateHasChanged();

        await Task.Delay(50);
        try
        {
            if (_jsModule != null)
            {
                await _jsModule.InvokeVoidAsync(
                    "scrollToBottom", 
                    _mainContentScroll);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Scroll error in parent: {ex.Message}");
        }
    }

    private async Task ConfirmDeleteCardAsync()
    {
        if (IsObserver)
        {
            return;
        }

        try
        {
            _isDeletingLocally = true;

            await CardService.DeleteCardAsync(Card.BoardId, Card.Id);

            MudDialog.Close(DialogResult.Ok(true));
        }
        catch (Exception ex)
        {
            _isDeletingLocally = false;
            Console.WriteLine($"Error deleting card: {ex.Message}");
        }
    }

    private async Task SaveTitleAsync()
    {
        if (IsObserver)
        {
            return;
        }

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
        if (IsObserver)
        {
            return;
        }

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

    private void Close() => MudDialog.Cancel();

    private void HandleCardDueDateUpdated(CardDueDateUpdateDto data)
    {
        if (data.CardId == Card.Id)
        {
            _dueDate = data.DueDate?.Date;
            Card.DueDate = data.DueDate?.Date;
            InvokeAsync(StateHasChanged);
        }
    }

    private void HandleCardMoved(CardMovedDto data)
    {
        if (Card.Id == data.CardId)
        {
            Card.SectionId = data.NewSectionId;
            Card.Status = data.NewSectionName;

            InvokeAsync(StateHasChanged);
        }
    }

    private void HandleDescriptionUpdated(CardDescriptionUpdateDto data)
    {
        if (Card.Id == data.CardId)
        {
            Card.Description = data.Description;

            _editedDescription = data.Description; 

            InvokeAsync(StateHasChanged);
        }
    }

    private void HandleCardRenamed(CardRenameDto data)
    {
        if (Card.Id == data.CardId)
        {
            Card.Name = data.NewTitle;

            _editedTitle = data.NewTitle;

            InvokeAsync(StateHasChanged);
        }
    }

    private void HandleCardDeleted(Guid cardId)
    {
        if (Card.Id == cardId && !_isDeletingLocally)
        {
            InvokeAsync(() =>
            {
                Snackbar.Add(
                    "This card was deleted by another user.", 
                    Severity.Warning);
                MudDialog.Cancel();
            });
        }
    }

    private void HandleActivityLogAdded(ActivityLogDto log)
    {
        if (log.CardId == Card.Id)
        {
            _activityLogs.Insert(0, log);
            InvokeAsync(StateHasChanged);
        }
    }

    public void Dispose()
    {
        _ = _jsModule?.DisposeAsync();

        BoardHubService.OnCardDueDateUpdated -= HandleCardDueDateUpdated;
        BoardHubService.OnCardMoved -= HandleCardMoved;
        BoardHubService.OnCardDescriptionUpdated -= HandleDescriptionUpdated;
        BoardHubService.OnCardRenamed -= HandleCardRenamed;
        BoardHubService.OnCardDeleted -= HandleCardDeleted;
        BoardHubService.OnActivityLogAdded -= HandleActivityLogAdded;
    }
}

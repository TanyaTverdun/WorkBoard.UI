using Microsoft.AspNetCore.Components;
using MudBlazor;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Requests;
using WorkBoard.Services.Abstraction.Services;
using WorkBoard.UI.ViewModels.Board;
using WorkBoard.UI.ViewModels.Comment;

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

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    [Inject]
    private ICommentService CommentService { get; set; } = default!;

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

    private List<CommentViewModel> _comments = new();

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
            LoadCommentsAsync()
        );
    }

    private async Task LoadCommentsAsync()
    {
        try
        {
            var commentsDto = await CommentService.GetCommentsByCardAsync(Card.Id);

            var uiComments = new List<CommentViewModel>();

            foreach (var dto in commentsDto)
            {
                var authorName = !string.IsNullOrWhiteSpace(dto.UserFullName) ? dto.UserFullName : "Unknown User";
                var initials = !string.IsNullOrWhiteSpace(dto.Initials) ? dto.Initials : "UU";

                uiComments.Add(new CommentViewModel
                {
                    Id = dto.Id,
                    AuthorName = authorName,
                    Initials = initials,
                    Date = dto.CreatedAt,
                    Text = dto.Text
                });
            }

            _comments = uiComments;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading comments: {ex.Message}");
            Snackbar.Add("Failed to load comments.", Severity.Error);
        }
        finally
        {
            StateHasChanged();
        }
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

    private void Close() => MudDialog.Cancel();


    /// /////////МОКИ///////////////////////////////////////////////////////////////////////////////
    private DateTime? _dueDate = null;

    private string _newComment = string.Empty;

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

    public record CardAttachmentMock(
        string FileName, 
        string FileSize, 
        string Icon, 
        Color IconColor);
}

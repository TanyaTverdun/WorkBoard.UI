using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Refit;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Hubs;
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

    private bool _isPendingDeleteCard = false;

    private DateTime? _dueDate;

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

    private int _commentsCount = 0;

    private List<AttachmentDto> _attachments = new();
    private MudFileUpload<IBrowserFile>? _fileUpload;
    private Guid? _pendingDeleteAttachmentId = null;

    private List<ActivityLogDto> _activityLogs = new();

    private ChecklistDto? _checklist;
    private List<LabelDto> _cardLabels = new();
    private List<CommentDto> _comments = new();

    private bool _isPickerLoaded = false;
    private MudDatePicker _datePicker;

    private IEnumerable<UserSearchDto> FilteredAssignableUsers =>
        string.IsNullOrWhiteSpace(_assigneeSearchText)
            ? _assignableUsers
            : _assignableUsers.Where(u =>
                u.FullName.Contains(_assigneeSearchText, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains(_assigneeSearchText, StringComparison.OrdinalIgnoreCase));

    protected override async Task OnInitializedAsync()
    {
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

    private async Task LoadAndOpenPicker()
    {
        _isPickerLoaded = true;
        StateHasChanged();

        await Task.Delay(50);

        _datePicker?.OpenAsync();
    }

    private string FormatBytes(long bytes)
    {
        string[] suffix = { "B", "KB", "MB", "GB", "TB" };
        int i;
        double dblSByte = bytes;
        for (i = 0; i < suffix.Length && bytes >= 1024; i++, bytes /= 1024)
        {
            dblSByte = bytes / 1024.0;
        }
        return $"{dblSByte:0.##} {suffix[i]}";
    }

    private (string Icon, Color Color) GetFileIconAndColor(string fileName)
    {
        var ext = System.IO.Path.GetExtension(fileName)?.ToLowerInvariant();
        return ext switch
        {
            ".pdf" => (Icons.Material.Filled.PictureAsPdf, Color.Error),
            ".doc" or ".docx" => (Icons.Material.Filled.Description, Color.Info),
            ".xls" or ".xlsx" or ".csv" => (Icons.Material.Filled.TableChart, Color.Success),
            ".png" or ".jpg" or ".jpeg" or ".gif" or ".svg" => (Icons.Material.Filled.Image, Color.Warning),
            ".zip" or ".rar" or ".7z" => (Icons.Material.Filled.Archive, Color.Secondary),
            _ => (Icons.Material.Filled.InsertDriveFile, Color.Default)
        };
    }

    private async Task OnFileSelected(InputFileChangeEventArgs e)
    {
        var file = e.File;
        if (file == null) return;

        await UploadFileAsync(file);
    }

    private async Task UploadFileAsync(IBrowserFile file)
    {
        if (file == null) return;

        long maxFileSize = 100L * 1024 * 1024;

        if (file.Size > maxFileSize)
        {
            Snackbar.Add(
                "File is too large. Maximum size is 100 MB.", 
                Severity.Error);

            return;
        }

        try
        {
            using var stream = file.OpenReadStream(maxFileSize);

            var streamPart = new StreamPart(
                stream, 
                file.Name, 
                file.ContentType);

            var uploadedAttachment = await AttachmentService.UploadAttachmentAsync(
                Card.Id, 
                streamPart);

            _attachments.Add(uploadedAttachment);

            Snackbar.Add(
                "File uploaded successfully", 
                Severity.Success);

            StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error uploading file: {ex.Message}");
            Snackbar.Add("Failed to upload file.", Severity.Error);
        }
        finally
        {
            if (_fileUpload != null)
            {
                await _fileUpload.ClearAsync();
            }

            StateHasChanged();
        }
    }

    private async Task ConfirmDeleteAttachmentAsync(Guid attachmentId)
    {
        try
        {
            await AttachmentService.DeleteAttachmentAsync(
                Card.Id, 
                attachmentId);

            _attachments.RemoveAll(a => a.Id == attachmentId);
            _pendingDeleteAttachmentId = null;

            Snackbar.Add("Attachment deleted", Severity.Success);
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting attachment: {ex.Message}");
            Snackbar.Add("Failed to delete attachment", Severity.Error);
        }
    }

    private async Task OnDueDateChangedAsync(DateTime? newDate)
    {
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

    private void UpdateCommentsCount(int count)
    {
        _commentsCount = count;
        StateHasChanged();
    }
    private async Task AddAssigneeAsync(UserSearchDto user)
    {
        try
        {
            var request = new AddCardAssigneeRequest(user.UserId);
            await CardService.AddCardAssigneeAsync(Card.Id, request);

            var newAssignee = new CardAssigneeDto(
                user.UserId,
                user.FullName,
                user.Email,
                user.AvatarUrl,
                user.Initials ?? "Un");

            _assignees.Add(newAssignee);
            StateHasChanged();
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

            _assignees.Remove(assignee);
            StateHasChanged();
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
        BoardHubService.OnActivityLogAdded -= HandleActivityLogAdded;
    }
}

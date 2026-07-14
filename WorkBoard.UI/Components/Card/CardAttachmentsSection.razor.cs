using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Refit;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Hubs;
using WorkBoard.Services.Abstraction.Services;

namespace WorkBoard.UI.Components.Card;

public partial class CardAttachmentsSection : ComponentBase, IDisposable
{
    [Parameter]
    public Guid CardId { get; set; }

    [Parameter]
    public List<AttachmentDto> Attachments { get; set; } = new();

    [Parameter]
    public EventCallback<List<AttachmentDto>> AttachmentsChanged { get; set; }

    [Inject]
    private IAttachmentService AttachmentService { get; set; } = default!;

    [Inject]
    private IBoardHubService BoardHubService { get; set; } = default!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    private List<AttachmentDto> _attachments = new();
    private Guid? _pendingDeleteAttachmentId = null;

    protected override void OnInitialized()
    {
        BoardHubService.OnAttachmentAdded += HandleAttachmentAdded;
        BoardHubService.OnAttachmentDeleted += HandleAttachmentDeleted;
    }

    protected override void OnParametersSet()
    {
        if (Attachments != null)
        {
            _attachments = Attachments.ToList();
        }
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

        long maxFileSize = 100L * 1024 * 1024; // 100 MB

        if (file.Size > maxFileSize)
        {
            Snackbar.Add("File is too large. Maximum size is 100 MB.", Severity.Error);
            return;
        }

        try
        {
            using var stream = file.OpenReadStream(maxFileSize);
            var streamPart = new StreamPart(stream, file.Name, file.ContentType);

            var uploadedAttachment = await AttachmentService.UploadAttachmentAsync(CardId, streamPart);

            _attachments.Add(uploadedAttachment);
            Attachments.Add(uploadedAttachment);

            await AttachmentsChanged.InvokeAsync(Attachments);

            Snackbar.Add("File uploaded successfully", Severity.Success);
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error uploading file: {ex.Message}");
            Snackbar.Add("Failed to upload file.", Severity.Error);
        }
    }

    private async Task ConfirmDeleteAttachmentAsync(Guid attachmentId)
    {
        try
        {
            await AttachmentService.DeleteAttachmentAsync(CardId, attachmentId);

            _attachments.RemoveAll(a => a.Id == attachmentId);
            Attachments.RemoveAll(a => a.Id == attachmentId);
            _pendingDeleteAttachmentId = null;

            await AttachmentsChanged.InvokeAsync(Attachments);

            Snackbar.Add("Attachment deleted", Severity.Success);
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting attachment: {ex.Message}");
            Snackbar.Add("Failed to delete attachment", Severity.Error);
        }
    }

    private void HandleAttachmentAdded(AttachmentAddedDto data)
    {
        if (CardId == data.CardId && !_attachments.Any(a => a.Id == data.Attachment.Id))
        {
            _attachments.Add(data.Attachment);
            Attachments.Add(data.Attachment);

            InvokeAsync(async () =>
            {
                await AttachmentsChanged.InvokeAsync(Attachments);
                StateHasChanged();
            });
        }
    }

    private void HandleAttachmentDeleted(AttachmentDeletedDto data)
    {
        if (CardId == data.CardId)
        {
            var removedCount = _attachments.RemoveAll(a => a.Id == data.AttachmentId);

            if (removedCount > 0)
            {
                Attachments.RemoveAll(a => a.Id == data.AttachmentId);

                if (_pendingDeleteAttachmentId == data.AttachmentId)
                {
                    _pendingDeleteAttachmentId = null;
                }

                InvokeAsync(async () =>
                {
                    await AttachmentsChanged.InvokeAsync(Attachments);
                    StateHasChanged();
                });
            }
        }
    }

    public void Dispose()
    {
        BoardHubService.OnAttachmentAdded -= HandleAttachmentAdded;
        BoardHubService.OnAttachmentDeleted -= HandleAttachmentDeleted;
    }
}

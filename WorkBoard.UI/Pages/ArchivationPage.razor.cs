using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using MudBlazor;
using WorkBoard.Domain.Enums;
using WorkBoard.Domain.Options;
using WorkBoard.Services.Abstraction.DTOs.Board;
using WorkBoard.Services.Abstraction.Hubs;
using WorkBoard.Services.Abstraction.Services;

namespace WorkBoard.UI.Pages;

public partial class ArchivationPage
{
    [Inject] 
    private IBoardService BoardService { get; set; } = null!;

    [Inject] 
    private ISnackbar Snackbar { get; set; } = null!;

    [Inject] 
    private IArchivationHubService ArchivationHubService { get; set; } = null!;

    [Inject]
    private IOptions<WorkBoardUiOptions> UiOptions { get; set; } = default!;

    private BoardArchiveStatus? _currentFilter = null;
    private List<BoardArchivationDto> _allBoards = new();
    private bool _isLoading = true;

    private IEnumerable<BoardArchivationDto> FilteredBoards =>
        _currentFilter.HasValue
            ? (_currentFilter.Value == BoardArchiveStatus.Pending
                ? _allBoards.Where(j => j.ArchiveStatus == BoardArchiveStatus.Pending || 
                j.ArchiveStatus == BoardArchiveStatus.Queued)
                : _allBoards.Where(j => j.ArchiveStatus == _currentFilter.Value))
            : _allBoards;

    protected override async Task OnInitializedAsync()
    {
        ArchivationHubService.OnArchivationStatusChanged += HandleArchivationStatusChanged;

        await LoadBoardsAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        var backendUrl = UiOptions.Value.BackendBaseUrl;

        try
        {
            await ArchivationHubService.StartConnectionAsync(backendUrl);
        }
        catch (Exception ex)
        {
            Snackbar.Add(
                "Working in offline mode. Live updates are unavailable", 
                Severity.Warning);
        }
    }

    private async Task LoadBoardsAsync()
    {
        try
        {
            _isLoading = true;
            var apiBoards = await BoardService.GetBoardsForArchivationAsync();

            _allBoards = apiBoards.ToList();
        }
        catch (Exception)
        {
            Snackbar.Add(
                "Failed to load boards for archivation", 
                Severity.Error);
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task OnArchiveClickAsync(Guid boardId)
    {
        try
        {
            await BoardService.ArchiveBoardAsync(boardId);

            var board = _allBoards.FirstOrDefault(j => j.Id == boardId);
            if (board != null)
            {
                board.ArchiveStatus = BoardArchiveStatus.Pending;
            }

            Snackbar.Add("Archivation started successfully", Severity.Success);
        }
        catch (Exception)
        {
            Snackbar.Add("Failed to start archivation", Severity.Error);
        }
    }

    private async Task OnRestoreClickAsync(Guid boardId)
    {
        try
        {
            await BoardService.RestoreBoardAsync(boardId);

            var board = _allBoards.FirstOrDefault(j => j.Id == boardId);
            if (board != null)
            {
                board.ArchiveStatus = BoardArchiveStatus.RestorePending;
            }

            Snackbar.Add(
                "Restore process started successfully", 
                Severity.Success);
        }
        catch (Exception)
        {
            Snackbar.Add(
                "Failed to start restoration", 
                Severity.Error);
        }
    }

    private int GetBoardsCount(BoardArchiveStatus status) => status == BoardArchiveStatus.Pending
        ? _allBoards.Count(b => b.ArchiveStatus == BoardArchiveStatus.Pending || 
        b.ArchiveStatus == BoardArchiveStatus.Queued)
        : _allBoards.Count(b => b.ArchiveStatus == status);

    private void SetFilter(BoardArchiveStatus status)
    {
        _currentFilter = _currentFilter == status ? null : status;
    }

    private void OnFilterChanged(BoardArchiveStatus? newFilter)
    {
        _currentFilter = newFilter;
    }

    private string GetBadgeText(BoardArchiveStatus status) => status switch
    {
        BoardArchiveStatus.Active => "Active",
        BoardArchiveStatus.Pending or BoardArchiveStatus.Queued => "Board Archiving...",
        BoardArchiveStatus.Archived => "Archived",
        BoardArchiveStatus.RestorePending => "Board restoring...",
        _ => "Unknown"
    };

    private string GetBadgeClass(BoardArchiveStatus status) => status switch
    {
        BoardArchiveStatus.Active => "badge-teal-dark",
        BoardArchiveStatus.Pending or BoardArchiveStatus.Queued => "badge-blue-dark",
        BoardArchiveStatus.Archived => "badge-green-dark",
        BoardArchiveStatus.RestorePending => "badge-orange-dark",
        _ => ""
    };

    private string GetRowBorderClass(BoardArchiveStatus status) => status switch
    {
        BoardArchiveStatus.Active => "border-teal-dark",
        BoardArchiveStatus.Pending or BoardArchiveStatus.Queued => "border-blue-dark",
        BoardArchiveStatus.Archived => "border-green-dark",
        BoardArchiveStatus.RestorePending => "border-orange-dark",
        _ => "border-default-dark"
    };

    private string GetRowIconBgClass(BoardArchiveStatus status) => status switch
    {
        BoardArchiveStatus.Active => "icon-bg-teal-dark",
        BoardArchiveStatus.Pending or BoardArchiveStatus.Queued => "icon-bg-blue-dark",
        BoardArchiveStatus.Archived => "icon-bg-green-dark",
        BoardArchiveStatus.RestorePending => "icon-bg-orange-dark",
        _ => ""
    };

    private string GetRowIconColorClass(BoardArchiveStatus status) => status switch
    {
        BoardArchiveStatus.Active => "icon-color-teal-dark",
        BoardArchiveStatus.Pending or BoardArchiveStatus.Queued => "icon-color-blue-dark",
        BoardArchiveStatus.Archived => "icon-color-green-dark",
        BoardArchiveStatus.RestorePending => "icon-color-orange-dark",
        _ => ""
    };

    private string GetRowIcon(BoardArchiveStatus status) => status switch
    {
        BoardArchiveStatus.Active => Icons.Material.Outlined.Storage,
        BoardArchiveStatus.Pending or BoardArchiveStatus.Queued => Icons.Material.Outlined.Sync,
        BoardArchiveStatus.Archived => Icons.Material.Outlined.CheckCircleOutline,
        BoardArchiveStatus.RestorePending => Icons.Material.Outlined.SettingsBackupRestore,
        _ => Icons.Material.Filled.Info
    };

    private void HandleArchivationStatusChanged(
        Guid boardId, 
        BoardArchiveStatus newStatus)
    {
        var board = _allBoards.FirstOrDefault(b => b.Id == boardId);
        if (board != null)
        {
            board.ArchiveStatus = newStatus;

            var actionText = newStatus == BoardArchiveStatus.Archived ? "archived" : "restored";
            Snackbar.Add($"Board successfully {actionText}!", Severity.Success);

            InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
    }

    public void Dispose()
    {
        ArchivationHubService.OnArchivationStatusChanged -= HandleArchivationStatusChanged;

        ArchivationHubService.StopConnectionAsync();

    }
}
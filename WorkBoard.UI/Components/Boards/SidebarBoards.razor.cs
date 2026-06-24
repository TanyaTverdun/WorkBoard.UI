using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using WorkBoard.Domain.Enums;
using WorkBoard.Services.Abstraction;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.StateProviders;

namespace WorkBoard.UI.Components.Boards;

public partial class SidebarBoards : ComponentBase, IDisposable
{
    [Inject]
    private IBoardService BoardService { get; set; } = null!;

    [Inject]
    private WorkspaceStateProvider WorkspaceStateProvider { get; set; } = null!;

    [Inject]
    private BoardStateService BoardStateService { get; set; } = default!;

    [Inject]
    protected NavigationManager NavigationManager { get; set; } = null!;

    protected bool CanManageBoards =>
            WorkspaceStateProvider.CurrentRole.HasValue &&
            WorkspaceStateProvider.CurrentRole.Value != WorkspaceRole.Observer;

    protected IReadOnlyList<BoardDto>? Boards { get; private set; }
    protected Guid? SelectedBoardId { get; private set; }
    protected bool _isDropdownOpen = true;

    protected bool _isCreateModalOpen;
    protected bool _isDeleteModalOpen;
    protected BoardDto? _boardToModify;

    protected Guid? WorkspaceId => WorkspaceStateProvider.SelectedWorkspaceId;

    protected override void OnInitialized()
    {
        WorkspaceStateProvider.OnWorkspaceChanged += HandleWorkspaceChanged;
        NavigationManager.LocationChanged += HandleLocationChanged;

        if (WorkspaceStateProvider.SelectedWorkspaceId.HasValue)
        {
            _ = LoadBoardsAsync(WorkspaceStateProvider.SelectedWorkspaceId.Value);
        }

        SyncSelectedBoardFromUrl();
    }

    private void SyncSelectedBoardFromUrl()
    {
        var uri = new Uri(NavigationManager.Uri);
        var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length >= 2 && segments[0].Equals("boards", StringComparison.OrdinalIgnoreCase))
        {
            if (Guid.TryParse(segments[1], out var boardId))
            {
                SelectedBoardId = boardId;
                return;
            }
        }

        SelectedBoardId = null;
    }

    private void HandleLocationChanged(
        object? sender, 
        LocationChangedEventArgs e)
    {
        SyncSelectedBoardFromUrl();
        InvokeAsync(StateHasChanged);
    }

    private async void HandleWorkspaceChanged(
        Guid? newWorkspaceId,
        WorkspaceRole? role)
    {
        if (newWorkspaceId.HasValue)
        {
            SelectedBoardId = null;
            await LoadBoardsAsync(newWorkspaceId.Value);
        }
        else
        {
            Boards = null;
            SelectedBoardId = null;
            NavigationManager.NavigateTo("/");
        }

        await InvokeAsync(StateHasChanged);
    }

    private async Task LoadBoardsAsync(Guid workspaceId)
    {
        try
        {
            Boards = await BoardService.GetWorkspaceBoardsAsync(workspaceId);

            if (SelectedBoardId == null && Boards != null && Boards.Any())
            {
                SelectBoard(Boards.First().Id);
            }
        }
        catch (Exception)
        {
            Boards = new List<BoardDto>();
        }
    }

    protected void ToggleDropdown()
    {
        _isDropdownOpen = !_isDropdownOpen;
    }

    private void SelectBoard(Guid id)
    {
        SelectedBoardId = id;

        var board = Boards?.FirstOrDefault(b => b.Id == id);
        if (board != null)
        {
            BoardStateService.SetBoardName(board.Name);
        }

        NavigationManager.NavigateTo($"/boards/{id}");
    }

    protected void OpenCreateModal()
    {
        _boardToModify = null;
        _isCreateModalOpen = true;
    }

    protected void OpenEditModal(BoardDto board)
    {
        _boardToModify = board;
        _isCreateModalOpen = true;
    }

    protected void OpenDeleteModal(BoardDto board)
    {
        _boardToModify = board;
        _isDeleteModalOpen = true;
    }

    protected void CloseCreateModal() => _isCreateModalOpen = false;
    protected void CloseDeleteModal() => _isDeleteModalOpen = false;

    protected async Task HandleBoardSavedAsync(Guid boardId)
    {
        _isCreateModalOpen = false;
        _boardToModify = null;

        if (WorkspaceId.HasValue)
        {
            await LoadBoardsAsync(WorkspaceId.Value);
            SelectBoard(boardId);
            StateHasChanged();
        }
    }

    protected async Task HandleBoardDeletedAsync()
    {
        if (_boardToModify != null && SelectedBoardId == _boardToModify.Id)
        {
            SelectedBoardId = null;
        }

        _isDeleteModalOpen = false;
        _boardToModify = null;

        if (WorkspaceId.HasValue)
        {
            await LoadBoardsAsync(WorkspaceId.Value);
            StateHasChanged();
        }
    }

    public void Dispose()
    {
        WorkspaceStateProvider.OnWorkspaceChanged -= HandleWorkspaceChanged;
        NavigationManager.LocationChanged -= HandleLocationChanged;
    }
}

using Microsoft.AspNetCore.Components;
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

        if (WorkspaceStateProvider.SelectedWorkspaceId.HasValue)
        {
            _ = LoadBoardsAsync(WorkspaceStateProvider.SelectedWorkspaceId.Value);
        }
    }

    private async void HandleWorkspaceChanged(
        Guid? newWorkspaceId,
        WorkspaceRole? role)
    {
        if (newWorkspaceId.HasValue)
        {
            await LoadBoardsAsync(newWorkspaceId.Value);
        }
        else
        {
            Boards = null;
            SelectedBoardId = null;
        }

        await InvokeAsync(StateHasChanged);
    }

    private async Task LoadBoardsAsync(Guid workspaceId)
    {
        try
        {
            Boards = await BoardService.GetWorkspaceBoardsAsync(workspaceId);
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
    }
}

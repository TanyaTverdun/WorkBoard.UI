using Microsoft.AspNetCore.Components;
using WorkBoard.Services.Abstraction.DTOs.Users;
using WorkBoard.Services.Abstraction.Services;
using WorkBoard.Services.StateProviders;

namespace WorkBoard.UI.Components.Boards.Modals;

public partial class DeleteBoardModal
{
    [Inject]
    private IBoardService BoardService { get; set; } = null!;

    [Inject]
    private WorkspaceStateProvider WorkspaceStateProvider { get; set; } = null!;

    [Parameter]
    public bool IsOpen { get; set; }

    [Parameter]
    public BoardDto? Board { get; set; }

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    public EventCallback OnBoardDeleted { get; set; }

    protected bool IsSubmitting { get; set; }
    protected bool _showErrorToast;
    protected string _errorMessage = string.Empty;

    protected override void OnParametersSet()
    {
        if (IsOpen)
        {
            IsSubmitting = false;
            _showErrorToast = false;
        }
    }

    protected async Task CloseAsync()
    {
        if (IsSubmitting)
        {
            return;
        }

        await OnClose.InvokeAsync();
    }

    protected void CloseToast()
    {
        _showErrorToast = false;
    }

    protected async Task HandleDeleteAsync()
    {
        if (IsSubmitting || Board == null)
        {
            return;
        }

        var activeWorkspaceId = WorkspaceStateProvider.SelectedWorkspaceId;
        if (!activeWorkspaceId.HasValue)
        {
            _errorMessage = "No active workspace selected.";
            _showErrorToast = true;
            return;
        }

        try
        {
            IsSubmitting = true;
            _showErrorToast = false;
            await BoardService.DeleteBoardAsync(activeWorkspaceId.Value, Board.Id);

            await OnBoardDeleted.InvokeAsync();
        }
        catch (Exception)
        {
            _errorMessage = "Failed to delete board";
            _showErrorToast = true;
            _ = AutoHideToastAsync();
        }
        finally
        {
            IsSubmitting = false;
        }
    }

    private async Task AutoHideToastAsync()
    {
        await Task.Delay(5000);
        _showErrorToast = false;
        StateHasChanged();
    }
}

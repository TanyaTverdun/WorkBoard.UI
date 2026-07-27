using Microsoft.AspNetCore.Components;
using WorkBoard.Services.Abstraction.Requests;
using WorkBoard.Services.Abstraction.Requests.Boards;
using WorkBoard.Services.Abstraction.Services;
using WorkBoard.Services.StateProviders;

namespace WorkBoard.UI.Components.Boards.Modals;

public partial class BoardModal
{
    [Inject]
    private IBoardService BoardService { get; set; } = null!;

    [Inject]
    private WorkspaceStateProvider WorkspaceStateProvider { get; set; } = null!;

    [Parameter]
    public bool IsOpen { get; set; }

    [Parameter]
    public Guid? BoardId { get; set; }

    [Parameter]
    public string? InitialName { get; set; }

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    public EventCallback<Guid> OnBoardSaved { get; set; }

    protected CreateBoardRequest Model { get; set; } = new();

    protected bool IsSubmitting { get; set; }
    protected bool _showErrorToast;
    protected string _errorMessage = string.Empty;

    protected bool IsEditMode => BoardId.HasValue;

    protected override void OnParametersSet()
    {
        if (IsOpen)
        {
            IsSubmitting = false;
            _showErrorToast = false;

            if (IsEditMode)
            {
                Model = new CreateBoardRequest
                {
                    Name = InitialName ?? string.Empty
                };
            }
            else
            {
                Model = new CreateBoardRequest();
            }
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

    protected async Task HandleValidSubmitAsync()
    {
        if (IsSubmitting)
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

            if (IsEditMode)
            {
                var updateRequest = new UpdateBoardRequest
                {
                    Name = Model.Name
                };

                await BoardService.UpdateBoardAsync(
                    activeWorkspaceId.Value,
                    BoardId!.Value,
                    updateRequest);

                await OnBoardSaved.InvokeAsync(BoardId.Value);
            }
            else
            {
                var createRequest = new CreateBoardRequest
                {
                    Name = Model.Name
                };

                var newBoardId = await BoardService.CreateBoardAsync(
                    activeWorkspaceId.Value,
                    createRequest);

                await OnBoardSaved.InvokeAsync(newBoardId);
            }
        }
        catch (Exception)
        {
            _errorMessage = IsEditMode ? "Failed to update board"
                : "Failed to create board";
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

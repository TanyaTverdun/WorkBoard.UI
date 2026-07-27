using Microsoft.AspNetCore.Components;
using WorkBoard.Services.Abstraction.DTOs.Users;
using WorkBoard.Services.Abstraction.Services;

namespace WorkBoard.UI.Components.Workspaces.Modal;

public partial class DeleteWorkspaceModal
{
    [Inject]
    private IWorkspaceService WorkspaceService { get; set; } = null!;

    [Parameter]
    public bool IsOpen { get; set; }

    [Parameter]
    public UserWorkspaceDto? Workspace { get; set; }

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    public EventCallback OnWorkspaceDeleted { get; set; }

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
        if (IsSubmitting || Workspace == null)
        {
            return;
        }

        try
        {
            IsSubmitting = true;
            _showErrorToast = false;

            await WorkspaceService.DeleteWorkspaceAsync(Workspace.Id);

            await OnWorkspaceDeleted.InvokeAsync();
        }
        catch (Exception)
        {
            _errorMessage = "Failed to delete workspace";
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

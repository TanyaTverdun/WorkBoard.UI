using Microsoft.AspNetCore.Components;
using WorkBoard.Services.Abstraction;
using WorkBoard.Services.Abstraction.Requests;

namespace WorkBoard.UI.Components.Workspaces.Modal;

public partial class CreateWorkspaceModal
{
    [Inject]
    private IWorkspaceService WorkspaceService { get; set; } = null!;

    [Parameter]
    public bool IsOpen { get; set; }

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    public EventCallback<Guid> OnWorkspaceCreated { get; set; }

    protected CreateWorkspaceRequest Model { get; set; } = new();
    protected bool IsSubmitting { get; set; }

    protected bool _showErrorToast;
    protected string _errorMessage = string.Empty;

    protected override void OnParametersSet()
    {
        if (IsOpen)
        {
            Model = new CreateWorkspaceRequest();
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

    protected async Task HandleValidSubmitAsync()
    {
        if (IsSubmitting)
        {
            return;
        }

        try
        {
            IsSubmitting = true;
            _showErrorToast = false;

            var newWorkspaceId = await WorkspaceService.CreateWorkspaceAsync(Model);

            await OnWorkspaceCreated.InvokeAsync(newWorkspaceId);
        }
        catch (Exception)
        {
            _errorMessage = "Failed to create workspace";
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

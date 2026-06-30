using Microsoft.AspNetCore.Components;
using WorkBoard.Services.Abstraction.Requests;
using WorkBoard.Services.Abstraction.Services;

namespace WorkBoard.UI.Components.Workspaces.Modal;

public partial class WorkspaceModal
{
    [Inject]
    private IWorkspaceService WorkspaceService { get; set; } = null!;

    [Parameter]
    public bool IsOpen { get; set; }

    [Parameter]
    public Guid? WorkspaceId { get; set; }

    [Parameter]
    public string? InitialName { get; set; }

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    public EventCallback<Guid> OnWorkspaceSaved { get; set; }
    protected CreateWorkspaceRequest Model { get; set; } = new();

    protected bool IsSubmitting { get; set; }
    protected bool _showErrorToast;
    protected string _errorMessage = string.Empty;

    protected bool IsEditMode => WorkspaceId.HasValue;

    protected override void OnParametersSet()
    {
        if (IsOpen)
        {
            IsSubmitting = false;
            _showErrorToast = false;

            if (IsEditMode)
            {
                Model = new CreateWorkspaceRequest 
                { 
                    Name = InitialName ?? string.Empty 
                };
            }
            else
            {
                Model = new CreateWorkspaceRequest();
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

        try
        {
            IsSubmitting = true;
            _showErrorToast = false;

            if (IsEditMode)
            {
                var updateRequest = new UpdateWorkspaceRequest 
                { 
                    Name = Model.Name 
                };

                await WorkspaceService.UpdateWorkspaceAsync(
                    WorkspaceId!.Value, 
                    updateRequest);

                await OnWorkspaceSaved.InvokeAsync(WorkspaceId.Value);
            }
            else
            {
                var newWorkspaceId = await WorkspaceService
                    .CreateWorkspaceAsync(Model);

                await OnWorkspaceSaved.InvokeAsync(newWorkspaceId);
            }
        }
        catch (Exception)
        {
            _errorMessage = IsEditMode ? "Failed to update workspace" 
                : "Failed to create workspace";
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

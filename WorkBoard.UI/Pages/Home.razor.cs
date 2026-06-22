using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using WorkBoard.Domain.Constants;
using WorkBoard.Services.Abstraction;
using WorkBoard.Services.StateProviders;

namespace WorkBoard.UI.Pages;

public partial class Home
{
    [Inject] 
    private NavigationManager Navigation { get; set; } = default!;
    [Inject] 
    private IWorkspaceService WorkspaceService { get; set; } = default!;
    [Inject] 
    private IBoardService BoardService { get; set; } = default!;
    [Inject] 
    private WorkspaceStateProvider WorkspaceStateProvider { get; set; } = default!;
    [CascadingParameter]
    private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

    private string Username { get; set; } = UiConstants.Auth.LoadingText;
    protected bool IsLoading { get; set; } = true;
    protected override async Task OnInitializedAsync()
    {
        if (AuthenticationStateTask is not null)
        {
            var authState = await AuthenticationStateTask;
            var user = authState.User;

            if (user.Identity is not null 
                && user.Identity.IsAuthenticated)
            {
                Username = user.Identity.Name 
                    ?? UiConstants.Auth.DefaultUsername;

                await PerformAutoRedirectAsync();
            }
            else
            {
                IsLoading = false;
            }
        }
        else
        {
            IsLoading = false;
        }
    }

    private async Task PerformAutoRedirectAsync()
    {
        try
        {
            var workspaces = await WorkspaceService.GetUserWorkspacesAsync();

            if (workspaces != null && workspaces.Any())
            {
                var firstWorkspace = workspaces.First();

                WorkspaceStateProvider.SetActiveWorkspace(
                    firstWorkspace.Id, 
                    firstWorkspace.UserRole);

                var boards = await BoardService.GetWorkspaceBoardsAsync(
                    firstWorkspace.Id);

                if (boards != null && boards.Any())
                {
                    var firstBoard = boards.First();

                    Navigation.NavigateTo($"/boards/{firstBoard.Id}");
                    return;
                }
            }

            IsLoading = false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Redirect failed: {ex.Message}");
            IsLoading = false;
        }
    }
}
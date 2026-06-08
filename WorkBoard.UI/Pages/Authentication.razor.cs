using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using WorkBoard.Domain.Constants;
using WorkBoard.Services.Abstraction;

namespace WorkBoard.UI.Pages;

public partial class Authentication
{
    [Inject] 
    private IAuthService AuthService { get; set; } = default!;
    [Inject] 
    private NavigationManager Navigation { get; set; } = default!;
    [Inject] 
    private ILogger<Authentication> Logger { get; set; } = default!;

    [Parameter] 
    public string? Action { get; set; }

    private async Task HandleLogInSuccess(RemoteAuthenticationState state)
    {
        var localUserId = await AuthService.AuthenticateUserInBackendAsync();

        if (localUserId != null)
        {
            Navigation.NavigateTo(AppRoutes.Home);
        }
        else
        {
            Logger.LogError("Failed to synchronize user with the backend.");
            Navigation.NavigateTo(AppRoutes.Login);
        }
    }
}
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using WorkBoard.Domain.Constants;

namespace WorkBoard.UI.Pages;

public partial class Login
{
    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    private string[] Features => UiConstants.Marketing.LoginFeatures;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity is not null && user.Identity.IsAuthenticated)
        {
            Navigation.NavigateTo(AppRoutes.Home);
        }
    }

    private void SignIn() => Navigation.NavigateTo(AppRoutes.Auth.LoginAction);
}
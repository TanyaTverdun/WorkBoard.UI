using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using WorkBoard.UI.Constants;

namespace WorkBoard.UI.Pages;

public partial class Home
{
    [CascadingParameter]
    private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

    private string Username { get; set; } = UiConstants.Auth.LoadingText;

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
            }
        }
    }
}
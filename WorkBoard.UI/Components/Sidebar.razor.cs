using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using WorkBoard.Domain.Constants;

namespace WorkBoard.UI.Components;

public partial class Sidebar
{
    [CascadingParameter]
    private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

    protected string Username { get; private set; } = UiConstants.Auth.LoadingText;
    protected string Initials { get; private set; } = string.Empty;

    protected string SearchQuery { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        if (AuthenticationStateTask is not null)
        {
            var authState = await AuthenticationStateTask;
            var user = authState.User;

            if (user.Identity is { IsAuthenticated: true })
            {
                Username = user.Identity.Name ?? UiConstants.Auth.DefaultUsername;

                SetInitials(Username);
            }
        }
    }

    private void SetInitials(string name)
    {
        if (string.IsNullOrWhiteSpace(name) ||
            name == UiConstants.Auth.LoadingText)
        {
            return;
        }

        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length >= 2)
        {
            Initials = $"{parts[0][0]}{parts[1][0]}".ToUpper();
        }
        else if (parts.Length == 1)
        {
            Initials = parts[0].Length >= 2
                ? parts[0][..2].ToUpper()
                : parts[0][0].ToString().ToUpper();
        }
    }
}

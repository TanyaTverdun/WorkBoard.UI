using Microsoft.AspNetCore.Components;
using WorkBoard.Domain.Constants;
using WorkBoard.Services.Abstraction.StateProviders;

namespace WorkBoard.UI.Components;

public partial class Sidebar
{
    [Inject]
    private ICurrentUserProvider CurrentUserProvider { get; set; } = default!;

    protected string Username { get; private set; } = UiConstants.Auth.LoadingText;
    protected string Initials { get; private set; } = string.Empty;
    protected string UserEmail { get; private set; } = string.Empty;
    protected string SearchQuery { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        var fullName = await CurrentUserProvider.GetFullNameAsync();
        var email = await CurrentUserProvider.GetEmailAsync();

        if (!string.IsNullOrEmpty(fullName))
        {
            Username = fullName;
            SetInitials(Username);
        }
        else
        {
            Username = UiConstants.Auth.DefaultUsername;
        }

        UserEmail = email ?? string.Empty;
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

    private bool _isProfileMenuOpen = false;

    private void ToggleProfileMenu()
    {
        _isProfileMenuOpen = !_isProfileMenuOpen;
    }

    private void GoToProfile()
    {
        _isProfileMenuOpen = false;
        // Твоя логіка переходу на сторінку профілю
        // NavigationManager.NavigateTo("/profile");
    }

    private async Task SignOut()
    {
        _isProfileMenuOpen = false;
        // Твоя логіка виходу (наприклад, очищення токену Azure AD)
        // await AuthService.LogoutAsync();
    }
}

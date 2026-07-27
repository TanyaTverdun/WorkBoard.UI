using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using WorkBoard.Services.Abstraction.DTOs.Users;
using WorkBoard.Services.Abstraction.Services;
using WorkBoard.Services.Abstraction.StateProviders;

namespace WorkBoard.Services.StateProviders;

public class CurrentUserProvider : ICurrentUserProvider
{
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly IUserService _userService;

    private UserProfileDto? _profile;
    public UserProfileDto? Profile => _profile;
    public event Action? OnProfileChanged;

    private const string AzureOidClaim =
        "http://schemas.microsoft.com/identity/claims/objectidentifier";
    private const string AzurePreferredUsernameClaim = "preferred_username";
    private const string AzureNameClaim = "name";

    public CurrentUserProvider(
        AuthenticationStateProvider authStateProvider,
        IUserService userService)
    {
        _authStateProvider = authStateProvider;
        _userService = userService;
    }

    public async Task LoadProfileAsync()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();

        if (authState.User.Identity?.IsAuthenticated == true)
        {
            _profile = await _userService.GetCurrentUserProfileAsync();
            OnProfileChanged?.Invoke();
        }
    }

    private async Task<ClaimsPrincipal> GetUserAsync()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        return authState.User;
    }

    public async Task<Guid?> GetUserIdAsync()
    {
        var user = await GetUserAsync();
        var nameIdentifier = user.FindFirst(AzureOidClaim)?.Value
                             ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(nameIdentifier, out var parsedGuid) ? parsedGuid : null;
    }

    public async Task<string?> GetEmailAsync()
    {
        var user = await GetUserAsync();
        return user.FindFirst(AzurePreferredUsernameClaim)?.Value
               ?? user.FindFirst(ClaimTypes.Email)?.Value;
    }

    public async Task<string?> GetFullNameAsync()
    {
        var user = await GetUserAsync();
        return user.FindFirst(AzureNameClaim)?.Value
               ?? user.FindFirst(ClaimTypes.Name)?.Value;
    }

    public void NotifyProfileChanged()
    {
        OnProfileChanged?.Invoke();
    }
}

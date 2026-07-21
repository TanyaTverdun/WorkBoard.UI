using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using WorkBoard.Services.Abstraction.StateProviders;

namespace WorkBoard.Services.StateProviders;

public class CurrentUserProvider : ICurrentUserProvider
{
    private readonly AuthenticationStateProvider _authStateProvider;

    private const string AzureOidClaim =
        "http://schemas.microsoft.com/identity/claims/objectidentifier";
    private const string AzurePreferredUsernameClaim = "preferred_username";
    private const string AzureNameClaim = "name";

    public CurrentUserProvider(AuthenticationStateProvider authStateProvider)
    {
        _authStateProvider = authStateProvider;
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
}

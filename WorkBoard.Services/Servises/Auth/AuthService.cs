using Microsoft.Extensions.Logging;
using Refit;
using WorkBoard.Services.Abstraction.Services;

namespace WorkBoard.Services.Servises.Auth;

internal class AuthService : IAuthService
{
    private readonly IAuthApi _authApi;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IAuthApi authApi, 
        ILogger<AuthService> logger)
    {
        _authApi = authApi;
        _logger = logger;
    }

    public async Task<Guid?> AuthenticateUserInBackendAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _authApi.AuthenticateUserInBackendAsync(
                cancellationToken);
        }
        catch (ApiException ex)
        {
            _logger.LogError(
                ex, 
                "Backend API returned an error status code: {StatusCode}", 
                ex.StatusCode);

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "Backend authentication failed with an unhandled exception");

            return null;
        }
    }
}

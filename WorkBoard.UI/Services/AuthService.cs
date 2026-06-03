using System.Net.Http.Json;
using WorkBoard.UI.Constants;

namespace WorkBoard.UI.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        HttpClient httpClient, 
        ILogger<AuthService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Guid?> AuthenticateUserInBackendAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsync(
                ApiEndpoints.Auth.Authenticate, 
                null, 
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var localUserId = await response.Content
                    .ReadFromJsonAsync<Guid>(
                        cancellationToken: cancellationToken);

                return localUserId;
            }

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

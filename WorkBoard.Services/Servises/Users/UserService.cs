using Microsoft.Extensions.Logging;
using Refit;
using WorkBoard.Services.Abstraction.DTOs.Users;
using WorkBoard.Services.Abstraction.Requests.Users;
using WorkBoard.Services.Abstraction.Services;

namespace WorkBoard.Services.Servises.Users;

internal class UserService : IUserService
{
    private readonly IUserApi _userApi;
    private readonly ILogger<UserService> _logger;

    public UserService(
       IUserApi userApi,
       ILogger<UserService> logger)
    {
        _userApi = userApi;
        _logger = logger;
    }

    public async Task<IReadOnlyList<UserSearchDto>> SearchAssignableUsersAsync(
        Guid boardId,
        string searchTerm,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _userApi.SearchAssignableUsersAsync(
                boardId,
                searchTerm,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while searching for assignable users in board {BoardId} " +
                "with term '{SearchTerm}'. Status: {StatusCode}",
                boardId,
                searchTerm,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while searching for assignable users in board {BoardId} " +
                " with term '{SearchTerm}'",
                boardId,
                searchTerm);
            throw;
        }
    }

    public async Task<UserProfileDto> GetCurrentUserProfileAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _userApi.GetCurrentUserProfileAsync(cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while fetching current user profile. Status: {StatusCode}",
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while fetching current user profile.");
            throw;
        }
    }

    public async Task UpdateAvatarColorAsync(
        string color,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new UpdateUserAvatarColorRequest
            {
                AvatarColor = color
            };

            await _userApi.UpdateAvatarColorAsync(request, cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while updating avatar color. Status: {StatusCode}",
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while updating avatar color.");
            throw;
        }
    }

    public async Task UploadAvatarImageAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var streamPart = new StreamPart(
                fileStream,
                fileName,
                contentType);

            await _userApi.UpdateAvatarImageAsync(
                streamPart,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while uploading avatar image. Status: {StatusCode}",
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while uploading avatar image.");
            throw;
        }
    }
}

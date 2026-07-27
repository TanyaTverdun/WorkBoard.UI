using WorkBoard.Services.Abstraction.DTOs.Users;

namespace WorkBoard.Services.Abstraction.Services;

public interface IUserService
{
    Task<IReadOnlyList<UserSearchDto>> SearchAssignableUsersAsync(
        Guid boardId,
        string searchTerm,
        CancellationToken cancellationToken = default);

    Task<UserProfileDto> GetCurrentUserProfileAsync(
        CancellationToken cancellationToken = default);

    Task UpdateAvatarColorAsync(
        string color,
        CancellationToken cancellationToken = default);

    Task UploadAvatarImageAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);
}

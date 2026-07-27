using Refit;
using WorkBoard.Services.Abstraction.DTOs.Users;
using WorkBoard.Services.Abstraction.Requests.Users;

namespace WorkBoard.Services.Servises.Users;

internal interface IUserApi
{
    [Get("/api/users/{boardId}/assignable-users")]
    Task<IReadOnlyList<UserSearchDto>> SearchAssignableUsersAsync(
        Guid boardId,
        [Query] string searchTerm,
        CancellationToken cancellationToken = default);

    [Get("/api/users/current-user")]
    Task<UserProfileDto> GetCurrentUserProfileAsync(
        CancellationToken cancellationToken = default);

    [Patch("/api/users/avatar-color")]
    Task UpdateAvatarColorAsync(
        [Body] UpdateUserAvatarColorRequest request,
        CancellationToken cancellationToken = default);

    [Multipart]
    [Patch("/api/users/avatar-image")]
    Task UpdateAvatarImageAsync(
        [AliasAs("file")] StreamPart file,
        CancellationToken cancellationToken = default);
}

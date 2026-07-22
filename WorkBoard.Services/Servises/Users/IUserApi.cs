using Refit;
using WorkBoard.Services.Abstraction.DTOs.Users;

namespace WorkBoard.Services.Servises.Users;

internal interface IUserApi
{
    [Get("/api/users/{boardId}/assignable-users")]
    Task<IReadOnlyList<UserSearchDto>> SearchAssignableUsersAsync(
        Guid boardId,
        [Query] string searchTerm,
        CancellationToken cancellationToken = default);
}

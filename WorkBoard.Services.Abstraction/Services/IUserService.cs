using WorkBoard.Services.Abstraction.DTOs.Users;

namespace WorkBoard.Services.Abstraction.Services;

public interface IUserService
{
    Task<IReadOnlyList<UserSearchDto>> SearchAssignableUsersAsync(
        Guid boardId,
        string searchTerm,
        CancellationToken cancellationToken = default);
}

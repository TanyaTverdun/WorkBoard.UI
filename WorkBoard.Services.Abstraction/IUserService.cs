using WorkBoard.Services.Abstraction.DTOs;

namespace WorkBoard.Services.Abstraction;

public interface IUserService
{
    Task<IReadOnlyList<UserSearchDto>> SearchAssignableUsersAsync(
        Guid boardId,
        string searchTerm,
        CancellationToken cancellationToken = default);
}

using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Requests;

namespace WorkBoard.Services.Abstraction;

public interface IBoardMembersService
{
    Task<IReadOnlyList<BoardMemberDto>> GetBoardMembersAsync(
        Guid workspaceId,
        Guid boardId,
        CancellationToken cancellationToken = default);

    Task AddBoardMemberAsync(
        Guid workspaceId,
        Guid boardId,
        AddMemberRequest request,
        CancellationToken cancellationToken = default);

    Task UpdateMemberRoleAsync(
        Guid workspaceId,
        Guid boardId,
        Guid userId,
        UpdateRoleRequest request,
        CancellationToken cancellationToken = default);

    Task RemoveBoardMemberAsync(
        Guid workspaceId,
        Guid boardId,
        Guid userId,
        CancellationToken cancellationToken = default);
}

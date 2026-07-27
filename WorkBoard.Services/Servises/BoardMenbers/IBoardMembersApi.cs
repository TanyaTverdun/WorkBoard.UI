using Refit;
using WorkBoard.Services.Abstraction.DTOs.BoardMembers;
using WorkBoard.Services.Abstraction.Requests.BoardMembers;

namespace WorkBoard.Services.Servises.BoardMenbers;

internal interface IBoardMembersApi
{
    [Get("/api/workspaces/{workspaceId}/boards/{boardId}/members")]
    Task<IReadOnlyList<BoardMemberDto>> GetBoardMembersAsync(
        Guid workspaceId,
        Guid boardId,
        CancellationToken cancellationToken = default);

    [Post("/api/workspaces/{workspaceId}/boards/{boardId}/members")]
    Task AddBoardMemberAsync(
        Guid workspaceId,
        Guid boardId,
        [Body] AddMemberRequest request,
        CancellationToken cancellationToken = default);

    [Patch("/api/workspaces/{workspaceId}/boards/{boardId}/members/{userId}/role")]
    Task UpdateMemberRoleAsync(
        Guid workspaceId,
        Guid boardId,
        Guid userId,
        [Body] UpdateRoleRequest request,
        CancellationToken cancellationToken = default);

    [Delete("/api/workspaces/{workspaceId}/boards/{boardId}/members/{userId}")]
    Task RemoveBoardMemberAsync(
        Guid workspaceId,
        Guid boardId,
        Guid userId,
        CancellationToken cancellationToken = default);
}

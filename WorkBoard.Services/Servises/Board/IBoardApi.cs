using Refit;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Requests;

namespace WorkBoard.Services.Servises.Board;

internal interface IBoardApi
{
    [Get("/api/workspaces/{workspaceId}/boards")]
    Task<IReadOnlyList<BoardDto>> GetWorkspaceBoardsAsync(
        Guid workspaceId,
        CancellationToken cancellationToken = default);

    [Post("/api/workspaces/{workspaceId}/boards")]
    Task<Guid> CreateBoardAsync(
        Guid workspaceId,
        [Body] CreateBoardRequest request,
        CancellationToken cancellationToken = default);

    [Put("/api/workspaces/{workspaceId}/boards/{boardId}")]
    Task UpdateBoardAsync(
        Guid workspaceId,
        Guid boardId,
        [Body] UpdateBoardRequest request,
        CancellationToken cancellationToken = default);

    [Delete("/api/workspaces/{workspaceId}/boards/{boardId}")]
    Task DeleteBoardAsync(
        Guid workspaceId,
        Guid boardId,
        CancellationToken cancellationToken = default);

    [Get("/api/workspaces/{workspaceId}/boards/{boardId}")]
    Task<BoardDto> GetBoardAsync(
        Guid workspaceId,
        Guid boardId,
        CancellationToken cancellationToken = default);
}

using Refit;
using WorkBoard.Services.Abstraction.DTOs.Board;
using WorkBoard.Services.Abstraction.DTOs.Users;
using WorkBoard.Services.Abstraction.Requests.Boards;

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

    [Post("/api/boards/{boardId}/archive")]
    Task ArchiveBoardAsync(
        Guid boardId,
        CancellationToken cancellationToken = default);

    [Post("/api/boards/{boardId}/restore")]
    Task RestoreBoardAsync(
        Guid boardId,
        CancellationToken cancellationToken = default);

    [Get("/api/boards/archivation")]
    Task<IReadOnlyList<BoardArchivationDto>> GetBoardsForArchivationAsync(
        CancellationToken cancellationToken = default);
}

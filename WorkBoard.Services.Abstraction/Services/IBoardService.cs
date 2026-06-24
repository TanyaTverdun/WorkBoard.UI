using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Requests;

namespace WorkBoard.Services.Abstraction.Services;

public interface IBoardService
{
    Task<IReadOnlyList<BoardDto>> GetWorkspaceBoardsAsync(
        Guid workspaceId,
        CancellationToken cancellationToken = default);

    Task<Guid> CreateBoardAsync(
        Guid workspaceId,
        CreateBoardRequest request,
        CancellationToken cancellationToken = default);

    Task UpdateBoardAsync(
        Guid workspaceId,
        Guid boardId,
        UpdateBoardRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteBoardAsync(
        Guid workspaceId,
        Guid boardId,
        CancellationToken cancellationToken = default);

    Task<BoardDto> GetBoardAsync(
        Guid workspaceId, 
        Guid boardId, 
        CancellationToken cancellationToken = default);
}

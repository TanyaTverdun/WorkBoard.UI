using Microsoft.Extensions.Logging;
using Refit;
using WorkBoard.Services.Abstraction.DTOs.Users;
using WorkBoard.Services.Abstraction.Requests.Boards;
using WorkBoard.Services.Abstraction.Services;

namespace WorkBoard.Services.Servises.Board;

internal class BoardService : IBoardService
{
    private readonly IBoardApi _boardApi;
    private readonly ILogger<BoardService> _logger;

    public BoardService(
        IBoardApi boardApi,
        ILogger<BoardService> logger)
    {
        _boardApi = boardApi;
        _logger = logger;
    }

    public async Task<IReadOnlyList<BoardDto>> GetWorkspaceBoardsAsync(
        Guid workspaceId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _boardApi.GetWorkspaceBoardsAsync(
                workspaceId, 
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while fetching boards for workspace " +
                "{WorkspaceId}. Status: {StatusCode}",
                workspaceId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while fetching boards for workspace " +
                "{WorkspaceId}",
                workspaceId);
            throw;
        }
    }

    public async Task<Guid> CreateBoardAsync(
        Guid workspaceId,
        CreateBoardRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _boardApi.CreateBoardAsync(
                workspaceId, 
                request, 
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while creating board '{BoardName}' " +
                "in workspace {WorkspaceId}. Status: {StatusCode}",
                request.Name,
                workspaceId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while creating board with name: " +
                "{BoardName} in workspace {WorkspaceId}",
                request.Name,
                workspaceId);
            throw;
        }
    }

    public async Task UpdateBoardAsync(
        Guid workspaceId,
        Guid boardId,
        UpdateBoardRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _boardApi.UpdateBoardAsync(
                workspaceId, 
                boardId, 
                request, 
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while updating board {BoardId} in workspace " +
                "{WorkspaceId}. Status: {StatusCode}",
                boardId,
                workspaceId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while updating board {BoardId} in " +
                "workspace {WorkspaceId}",
                boardId,
                workspaceId);
            throw;
        }
    }

    public async Task DeleteBoardAsync(
        Guid workspaceId,
        Guid boardId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _boardApi.DeleteBoardAsync(
                workspaceId, 
                boardId, 
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while deleting board {BoardId} from workspace " +
                "{WorkspaceId}. Status: {StatusCode}",
                boardId,
                workspaceId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while deleting board {BoardId} " +
                "from workspace {WorkspaceId}",
                boardId,
                workspaceId);
            throw;
        }
    }

    public async Task<BoardDto> GetBoardAsync(
        Guid workspaceId,
        Guid boardId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _boardApi.GetBoardAsync(
                workspaceId, 
                boardId, 
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while fetching board {BoardId} in workspace " +
                "{WorkspaceId}. Status: {StatusCode}",
                boardId,
                workspaceId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while fetching board {BoardId} in " +
                "workspace {WorkspaceId}",
                boardId,
                workspaceId);
            throw;
        }
    }
}

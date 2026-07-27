using Microsoft.Extensions.Logging;
using Refit;
using WorkBoard.Services.Abstraction.DTOs.BoardMembers;
using WorkBoard.Services.Abstraction.Requests.BoardMembers;
using WorkBoard.Services.Abstraction.Services;

namespace WorkBoard.Services.Servises.BoardMenbers;

internal class BoardMembersService : IBoardMembersService
{
    private readonly IBoardMembersApi _boardMembersApi;
    private readonly ILogger<BoardMembersService> _logger;

    public BoardMembersService(
        IBoardMembersApi boardMembersApi,
        ILogger<BoardMembersService> logger)
    {
        _boardMembersApi = boardMembersApi;
        _logger = logger;
    }

    public async Task<IReadOnlyList<BoardMemberDto>> GetBoardMembersAsync(
        Guid workspaceId,
        Guid boardId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _boardMembersApi.GetBoardMembersAsync(
                workspaceId,
                boardId,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while fetching members for board {BoardId} " +
                "in workspace {WorkspaceId}. Status: {StatusCode}",
                boardId,
                workspaceId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while fetching members for board {BoardId} " +
                "in workspace {WorkspaceId}",
                boardId,
                workspaceId);
            throw;
        }
    }

    public async Task AddBoardMemberAsync(
        Guid workspaceId,
        Guid boardId,
        AddMemberRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _boardMembersApi.AddBoardMemberAsync(
                workspaceId,
                boardId,
                request,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while adding user {UserId} to board {BoardId} " +
                "in workspace {WorkspaceId}. Status: {StatusCode}",
                request.UserId,
                boardId,
                workspaceId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while adding user {UserId} to board {BoardId} " +
                "in workspace {WorkspaceId}",
                request.UserId,
                boardId,
                workspaceId);
            throw;
        }
    }

    public async Task UpdateMemberRoleAsync(
        Guid workspaceId,
        Guid boardId,
        Guid userId,
        UpdateRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _boardMembersApi.UpdateMemberRoleAsync(
                workspaceId,
                boardId,
                userId,
                request,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while updating role for user {UserId} in board {BoardId} " +
                "in workspace {WorkspaceId}. Status: {StatusCode}",
                userId,
                boardId,
                workspaceId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while updating role for user {UserId} in board {BoardId} " +
                "in workspace {WorkspaceId}",
                userId,
                boardId,
                workspaceId);
            throw;
        }
    }

    public async Task RemoveBoardMemberAsync(
        Guid workspaceId,
        Guid boardId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _boardMembersApi.RemoveBoardMemberAsync(
                workspaceId,
                boardId,
                userId,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while removing user {UserId} from board {BoardId} " +
                "in workspace {WorkspaceId}. Status: {StatusCode}",
                userId,
                boardId,
                workspaceId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while removing user {UserId} from board {BoardId} " +
                "in workspace {WorkspaceId}",
                userId,
                boardId,
                workspaceId);
            throw;
        }
    }
}

using Microsoft.Extensions.Logging;
using Refit;
using WorkBoard.Services.Abstraction.DTOs.Users;
using WorkBoard.Services.Abstraction.DTOs.Workspaces;
using WorkBoard.Services.Abstraction.Requests.Workspaces;
using WorkBoard.Services.Abstraction.Services;

namespace WorkBoard.Services.Servises.Workspace;

internal class WorkspaceService : IWorkspaceService
{
    private readonly IWorkspaceApi _workspaceApi;
    private readonly ILogger<WorkspaceService> _logger;

    public WorkspaceService(
        IWorkspaceApi workspaceApi, 
        ILogger<WorkspaceService> logger)
    {
        _workspaceApi = workspaceApi;
        _logger = logger;
    }

    public async Task<IReadOnlyList<UserWorkspaceDto>> GetUserWorkspacesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _workspaceApi.GetMyWorkspacesAsync(
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx, 
                "API error occurred while fetching workspaces. " +
                "Status: {StatusCode}", 
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "Error occurred while fetching user workspaces");
            throw;
        }
    }

    public async Task<Guid> CreateWorkspaceAsync(
        CreateWorkspaceRequest request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _workspaceApi.CreateWorkspaceAsync(
                request, 
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx, 
                "API error occurred while creating workspace " +
                "'{WorkspaceName}'. Status: {StatusCode}",
                request.Name, apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "Error occurred while creating workspace " +
                "with name: {WorkspaceName}", 
                request.Name);
            throw;
        }
    }

    public async Task UpdateWorkspaceAsync(
        Guid id, 
        UpdateWorkspaceRequest request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _workspaceApi.UpdateWorkspaceAsync(
                id, 
                request, 
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx, 
                "API error occurred while updating workspace " +
                "{WorkspaceId}. Status: {StatusCode}", 
                id, 
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "Error occurred while updating workspace {WorkspaceId}", 
                id);
            throw;
        }
    }

    public async Task DeleteWorkspaceAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _workspaceApi.DeleteWorkspaceAsync(
                id, 
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx, 
                "API error occurred while deleting workspace " +
                "{WorkspaceId}. Status: {StatusCode}", 
                id, 
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "Error occurred while deleting workspace {WorkspaceId}", 
                id);
            throw;
        }
    }

    public async Task<IReadOnlyList<UserWorkspaceDto>> GetWorkspacesForRoleManagementAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _workspaceApi.GetWorkspacesForRoleManagementAsync(
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while fetching workspaces for role management. " +
                "Status: {StatusCode}",
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while fetching workspaces for role management.");
            throw;
        }
    }

    public async Task UpdateWorkspaceMemberRoleAsync(
        Guid workspaceId,
        Guid memberId,
        UpdateWorkspaceMemberRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _workspaceApi.UpdateWorkspaceMemberRoleAsync(
                workspaceId,
                memberId,
                request,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while updating role for member {MemberId} " +
                "in workspace {WorkspaceId}. Status: {StatusCode}",
                memberId,
                workspaceId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while updating role for member {MemberId} " +
                "in workspace {WorkspaceId}",
                memberId,
                workspaceId);
            throw;
        }
    }

    public async Task AddWorkspaceMemberAsync(
        Guid workspaceId,
        AddWorkspaceMemberRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _workspaceApi.AddWorkspaceMemberAsync(
                workspaceId,
                request,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while adding member {UserId} " +
                "to workspace {WorkspaceId}. Status: {StatusCode}",
                request.UserId,
                workspaceId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while adding member {UserId} " +
                "to workspace {WorkspaceId}",
                request.UserId,
                workspaceId);
            throw;
        }
    }

    public async Task RemoveWorkspaceMemberAsync(
        Guid workspaceId,
        Guid memberId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _workspaceApi.RemoveWorkspaceMemberAsync(
                workspaceId,
                memberId,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while removing member {MemberId} " +
                "from workspace {WorkspaceId}. Status: {StatusCode}",
                memberId,
                workspaceId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while removing member {MemberId} " +
                "from workspace {WorkspaceId}",
                memberId,
                workspaceId);
            throw;
        }
    }

    public async Task<IReadOnlyList<WorkspaceMemberDto>> GetWorkspaceMembersAsync(
        Guid workspaceId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _workspaceApi.GetWorkspaceMembersAsync(
                workspaceId,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while fetching " +
                "members for workspace {WorkspaceId}. " +
                "Status: {StatusCode}",
                workspaceId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while fetching members " +
                "for workspace {WorkspaceId}",
                workspaceId);
            throw;
        }
    }

    public async Task<IReadOnlyList<UserSearchDto>> SearchAssignableUsersAsync(
        Guid workspaceId,
        string searchTerm,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _workspaceApi.SearchAssignableUsersAsync(
                workspaceId,
                searchTerm,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while searching assignable " +
                "users for workspace {WorkspaceId}. " +
                "Status: {StatusCode}",
                workspaceId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while searching assignable users " +
                "for workspace {WorkspaceId}",
                workspaceId);
            throw;
        }
    }
}

using Microsoft.Extensions.Logging;
using Refit;
using WorkBoard.Services.Abstraction;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Requests;

namespace WorkBoard.Services.Workspace;

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
}

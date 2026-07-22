using Microsoft.Extensions.Logging;
using Refit;
using WorkBoard.Services.Abstraction.DTOs.Checklists;
using WorkBoard.Services.Abstraction.Requests.Checklists;
using WorkBoard.Services.Abstraction.Services;

namespace WorkBoard.Services.Servises.Checklist;

internal class ChecklistService : IChecklistService
{
    private readonly IChecklistApi _checklistApi;
    private readonly ILogger<ChecklistService> _logger;

    public ChecklistService(
        IChecklistApi checklistApi,
        ILogger<ChecklistService> logger)
    {
        _checklistApi = checklistApi;
        _logger = logger;
    }

    public async Task<ChecklistDto> CreateChecklistAsync(
        Guid cardId,
        CreateChecklistRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _checklistApi.CreateChecklistAsync(
                cardId,
                request,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while creating checklist '{Name}'" +
                " for card {CardId}. Status: {StatusCode}",
                request.Name,
                cardId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while creating checklist '{Name}'" +
                " for card {CardId}",
                request.Name,
                cardId);
            throw;
        }
    }

    public async Task<ChecklistDto> UpdateChecklistAsync(
        Guid checklistId,
        UpdateChecklistRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _checklistApi.UpdateChecklistAsync(
                checklistId,
                request,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while updating checklist {ChecklistId}." +
                " Status: {StatusCode}",
                checklistId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while updating checklist {ChecklistId}",
                checklistId);
            throw;
        }
    }

    public async Task DeleteChecklistAsync(
        Guid checklistId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _checklistApi.DeleteChecklistAsync(
                checklistId,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while deleting checklist {ChecklistId}." +
                " Status: {StatusCode}",
                checklistId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while deleting checklist {ChecklistId}",
                checklistId);
            throw;
        }
    }

    public async Task<ChecklistItemDto> AddChecklistItemAsync(
        Guid checklistId,
        AddChecklistItemRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _checklistApi.AddChecklistItemAsync(
                checklistId,
                request,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while adding item '{Title}' to checklist " +
                "{ChecklistId}. Status: {StatusCode}",
                request.Title,
                checklistId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while adding item '{Title}' " +
                "to checklist {ChecklistId}",
                request.Title,
                checklistId);
            throw;
        }
    }

    public async Task<ChecklistItemDto> UpdateChecklistItemStatusAsync(
        Guid itemId,
        UpdateChecklistItemStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _checklistApi.UpdateChecklistItemStatusAsync(
                itemId,
                request,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while updating status " +
                "for item {ItemId}. Status: {StatusCode}",
                itemId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while updating status " +
                "for item {ItemId}",
                itemId);
            throw;
        }
    }

    public async Task<ChecklistItemDto> UpdateChecklistItemAsync(
        Guid itemId,
        UpdateChecklistItemRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _checklistApi.UpdateChecklistItemAsync(
                itemId,
                request,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while updating title " +
                "for item {ItemId}. Status: {StatusCode}",
                itemId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while updating title " +
                "for item {ItemId}",
                itemId);
            throw;
        }
    }

    public async Task DeleteChecklistItemAsync(
        Guid itemId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _checklistApi.DeleteChecklistItemAsync(
                itemId,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while deleting item {ItemId}. Status: {StatusCode}",
                itemId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while deleting item {ItemId}",
                itemId);
            throw;
        }
    }
}

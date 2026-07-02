using Microsoft.Extensions.Logging;
using Refit;
using WorkBoard.Services.Abstraction.DTOs;
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

    public async Task<IReadOnlyList<ChecklistDto>> GetChecklistsByCardAsync(
        Guid cardId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _checklistApi.GetChecklistsByCardAsync(
                cardId,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while fetching checklists " +
                "for card {CardId}. Status: {StatusCode}",
                cardId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while fetching checklists " +
                "for card {CardId}",
                cardId);
            throw;
        }
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
}

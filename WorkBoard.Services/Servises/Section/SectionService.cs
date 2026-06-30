using Microsoft.Extensions.Logging;
using Refit;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Requests;
using WorkBoard.Services.Abstraction.Services;

namespace WorkBoard.Services.Servises.Section;

internal class SectionService : ISectionService
{
    private readonly ISectionApi _sectionApi;
    private readonly ILogger<SectionService> _logger;

    public SectionService(
        ISectionApi sectionApi,
        ILogger<SectionService> logger)
    {
        _sectionApi = sectionApi;
        _logger = logger;
    }

    public async Task<IReadOnlyList<SectionDto>> GetSectionsByBoardAsync(
        Guid boardId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _sectionApi.GetSectionsByBoardAsync(
                boardId,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while fetching sections " +
                "for board {BoardId}. Status: {StatusCode}",
                boardId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while fetching sections for board {BoardId}",
                boardId);
            throw;
        }
    }

    public async Task<Guid> CreateSectionAsync(
        Guid boardId,
        CreateSectionRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _sectionApi.CreateSectionAsync(
                boardId,
                request,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while creating section '{Name}' " +
                "for board {BoardId}. Status: {StatusCode}",
                request.Name,
                boardId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while creating section '{Name}' " +
                "for board {BoardId}",
                request.Name,
                boardId);
            throw;
        }
    }

    public async Task RenameSectionAsync(
        Guid boardId,
        Guid sectionId,
        UpdateSectionNameRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _sectionApi.RenameSectionAsync(
                boardId,
                sectionId,
                request,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while renaming section {SectionId} " +
                "on board {BoardId}. Status: {StatusCode}",
                sectionId,
                boardId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while renaming section {SectionId} on board {BoardId}",
                sectionId,
                boardId);
            throw;
        }
    }

    public async Task DeleteSectionAsync(
        Guid boardId,
        Guid sectionId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _sectionApi.DeleteSectionAsync(
                boardId,
                sectionId,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while deleting section {SectionId} " +
                "from board {BoardId}. Status: {StatusCode}",
                sectionId,
                boardId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while deleting section {SectionId} " +
                "from board {BoardId}",
                sectionId,
                boardId);
            throw;
        }
    }

    public async Task MoveSectionAsync(
        Guid boardId, 
        Guid sectionId, 
        MoveSectionRequest request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _sectionApi.MoveSectionAsync(
                boardId,
                sectionId,
                request,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while moving section {SectionId} " +
                "on board {BoardId} to position {NewPosition}. " +
                "Status: {StatusCode}",
                sectionId,
                boardId,
                request.NewPosition,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while moving section {SectionId} " +
                "on board {BoardId} to position {NewPosition}",
                sectionId,
                boardId,
                request.NewPosition);
            throw;
        }
    }
}
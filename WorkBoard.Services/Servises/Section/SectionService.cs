using Microsoft.Extensions.Logging;
using WorkBoard.Services.Abstraction;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Requests;

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
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "Error fetching sections for board {BoardId}", 
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
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "Error creating section '{Name}' for board {BoardId}", 
                request.Name, 
                boardId);

            throw;
        }
    }
}

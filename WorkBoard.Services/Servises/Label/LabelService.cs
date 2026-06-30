using Microsoft.Extensions.Logging;
using Refit;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Services;

namespace WorkBoard.Services.Servises.Label;

internal class LabelService : ILabelService
{
    private readonly ILabelApi _labelApi;
    private readonly ILogger<LabelService> _logger;

    public LabelService(
        ILabelApi labelApi,
        ILogger<LabelService> logger)
    {
        _labelApi = labelApi;
        _logger = logger;
    }

    public async Task<IReadOnlyList<LabelDto>> GetLabelsByBoardAsync(
        Guid boardId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _labelApi.GetLabelsByBoardAsync(
                boardId,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while fetching labels " +
                "for board {BoardId}. Status: {StatusCode}",
                boardId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while fetching labels for board {BoardId}",
                boardId);
            throw;
        }
    }

    public async Task<LabelDto> CreateLabelAsync(
        Guid cardId,
        CreateLabelRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _labelApi.CreateLabelAsync(
                cardId,
                request,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while creating label '{Name}' " +
                "for card {CardId}. Status: {StatusCode}",
                request.Name,
                cardId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while creating label '{Name}' " +
                "for card {CardId}",
                request.Name,
                cardId);
            throw;
        }
    }
}

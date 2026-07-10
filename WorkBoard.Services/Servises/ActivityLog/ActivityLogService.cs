using Microsoft.Extensions.Logging;
using Refit;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Services;

namespace WorkBoard.Services.Servises.ActivityLog;

public class ActivityLogService : IActivityLogService
{
    private readonly IActivityLogApi _activityLogApi;
    private readonly ILogger<ActivityLogService> _logger;

    public ActivityLogService(
        IActivityLogApi activityLogApi,
        ILogger<ActivityLogService> logger)
    {
        _activityLogApi = activityLogApi;
        _logger = logger;
    }

    public async Task<IEnumerable<ActivityLogDto>> GetActivityLogsByCardAsync(
        Guid cardId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _activityLogApi.GetActivityLogsByCardAsync(
                cardId,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error getting activity logs for card {CardId}. " +
                "Status: {StatusCode}",
                cardId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error getting activity logs for card {CardId}",
                cardId);
            throw;
        }
    }
}

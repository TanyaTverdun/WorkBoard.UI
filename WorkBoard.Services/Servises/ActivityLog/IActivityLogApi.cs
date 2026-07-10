using Refit;
using WorkBoard.Services.Abstraction.DTOs;

namespace WorkBoard.Services.Servises.ActivityLog;

public interface IActivityLogApi
{
    [Get("/api/cards/{cardId}/activity-logs")]
    Task<IEnumerable<ActivityLogDto>> GetActivityLogsByCardAsync(
        Guid cardId,
        CancellationToken cancellationToken = default);
}

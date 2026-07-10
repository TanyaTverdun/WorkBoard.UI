using WorkBoard.Services.Abstraction.DTOs;

namespace WorkBoard.Services.Abstraction.Services;

public interface IActivityLogService
{
    Task<IEnumerable<ActivityLogDto>> GetActivityLogsByCardAsync(
        Guid cardId,
        CancellationToken cancellationToken = default);
}

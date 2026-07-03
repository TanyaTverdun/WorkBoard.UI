using WorkBoard.Services.Abstraction.DTOs;

namespace WorkBoard.Services.Abstraction.Services;

public interface IChecklistService
{
    Task<ChecklistDto> GetChecklistByCardAsync(
        Guid cardId,
        CancellationToken cancellationToken = default);

    Task<ChecklistDto> CreateChecklistAsync(
        Guid cardId,
        CreateChecklistRequest request,
        CancellationToken cancellationToken = default);

    Task<ChecklistDto> UpdateChecklistAsync(
        Guid checklistId,
        UpdateChecklistRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteChecklistAsync(
        Guid checklistId,
        CancellationToken cancellationToken = default);
}

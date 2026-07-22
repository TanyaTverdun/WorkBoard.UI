using WorkBoard.Services.Abstraction.DTOs.Checklists;
using WorkBoard.Services.Abstraction.Requests.Checklists;

namespace WorkBoard.Services.Abstraction.Services;

public interface IChecklistService
{
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

    Task<ChecklistItemDto> AddChecklistItemAsync(
        Guid checklistId,
        AddChecklistItemRequest request,
        CancellationToken cancellationToken = default);

    Task<ChecklistItemDto> UpdateChecklistItemStatusAsync(
        Guid itemId,
        UpdateChecklistItemStatusRequest request,
        CancellationToken cancellationToken = default);

    Task<ChecklistItemDto> UpdateChecklistItemAsync(
        Guid itemId,
        UpdateChecklistItemRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteChecklistItemAsync(
        Guid itemId,
        CancellationToken cancellationToken = default);
}

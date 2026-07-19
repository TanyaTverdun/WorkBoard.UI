using Refit;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Requests;

namespace WorkBoard.Services.Servises.Checklist;

internal interface IChecklistApi
{
    [Post("/api/cards/{cardId}/checklists")]
    Task<ChecklistDto> CreateChecklistAsync(
        Guid cardId,
        [Body] CreateChecklistRequest request,
        CancellationToken cancellationToken = default);

    [Put("/api/checklists/{checklistId}")]
    Task<ChecklistDto> UpdateChecklistAsync(
        Guid checklistId,
        [Body] UpdateChecklistRequest request,
        CancellationToken cancellationToken = default);

    [Delete("/api/checklists/{checklistId}")]
    Task DeleteChecklistAsync(
        Guid checklistId,
        CancellationToken cancellationToken = default);

    [Post("/api/checklists/{checklistId}/items")]
    Task<ChecklistItemDto> AddChecklistItemAsync(
        Guid checklistId,
        [Body] AddChecklistItemRequest request,
        CancellationToken cancellationToken = default);

    [Put("/api/checklists/items/{itemId}/status")]
    Task<ChecklistItemDto> UpdateChecklistItemStatusAsync(
        Guid itemId,
        [Body] UpdateChecklistItemStatusRequest request,
        CancellationToken cancellationToken = default);

    [Put("/api/checklists/items/{itemId}")]
    Task<ChecklistItemDto> UpdateChecklistItemAsync(
        Guid itemId,
        [Body] UpdateChecklistItemRequest request,
        CancellationToken cancellationToken = default);

    [Delete("/api/checklists/items/{itemId}")]
    Task DeleteChecklistItemAsync(
        Guid itemId,
        CancellationToken cancellationToken = default);
}

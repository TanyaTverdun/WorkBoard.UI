using Refit;
using WorkBoard.Services.Abstraction.DTOs;

namespace WorkBoard.Services.Servises.Checklist;

internal interface IChecklistApi
{
    [Get("/api/cards/{cardId}/checklists")]
    Task<IReadOnlyList<ChecklistDto>> GetChecklistsByCardAsync(
        Guid cardId,
        CancellationToken cancellationToken = default);

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
}

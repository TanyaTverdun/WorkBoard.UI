using Refit;
using WorkBoard.Services.Abstraction.DTOs.Labels;
using WorkBoard.Services.Abstraction.Requests.Labels;

namespace WorkBoard.Services.Servises.Label;

internal interface ILabelApi
{
    [Get("/api/boards/{boardId}/labels")]
    Task<IReadOnlyList<LabelDto>> GetLabelsByBoardAsync(
        Guid boardId,
        CancellationToken cancellationToken = default);

    [Post("/api/cards/{cardId}/labels")]
    Task<LabelDto> CreateLabelAsync(
        Guid cardId,
        [Body] CreateLabelRequest request,
        CancellationToken cancellationToken = default);

    [Post("/api/cards/{cardId}/labels/{labelId}")]
    Task AddLabelToCardAsync(
        Guid cardId,
        Guid labelId,
        CancellationToken cancellationToken = default);

    [Delete("/api/cards/{cardId}/labels/{labelId}")]
    Task RemoveLabelFromCardAsync(
        Guid cardId,
        Guid labelId,
        CancellationToken cancellationToken = default);

    [Put("/api/labels/{labelId}")]
    Task<LabelDto> UpdateLabelAsync(
        Guid labelId,
        [Body] UpdateLabelRequest request,
        CancellationToken cancellationToken = default);

    [Delete("/api/labels/{labelId}")]
    Task DeleteLabelAsync(
        Guid labelId,
        CancellationToken cancellationToken = default);
}

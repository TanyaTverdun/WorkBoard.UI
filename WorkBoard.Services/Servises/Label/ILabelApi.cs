using Refit;
using WorkBoard.Services.Abstraction.DTOs;

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
}

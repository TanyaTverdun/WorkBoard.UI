using WorkBoard.Services.Abstraction.DTOs.Labels;
using WorkBoard.Services.Abstraction.Requests.Labels;

namespace WorkBoard.Services.Abstraction.Services;

public interface ILabelService
{
    Task<IReadOnlyList<LabelDto>> GetLabelsByBoardAsync(
        Guid boardId,
        CancellationToken cancellationToken = default);

    Task<LabelDto> CreateLabelAsync(
        Guid cardId,
        CreateLabelRequest request,
        CancellationToken cancellationToken = default);

    Task AddLabelToCardAsync(
        Guid cardId,
        Guid labelId,
        CancellationToken cancellationToken = default);

    Task RemoveLabelFromCardAsync(
        Guid cardId,
        Guid labelId,
        CancellationToken cancellationToken = default);

    Task<LabelDto> UpdateLabelAsync(
        Guid labelId,
        UpdateLabelRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteLabelAsync(
        Guid labelId,
        CancellationToken cancellationToken = default);
}

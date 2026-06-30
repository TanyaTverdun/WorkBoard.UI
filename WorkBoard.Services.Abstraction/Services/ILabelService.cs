using WorkBoard.Services.Abstraction.DTOs;

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
}

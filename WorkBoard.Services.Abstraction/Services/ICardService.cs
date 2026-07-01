using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Requests;
using WorkBoard.Services.Abstraction.Requestsж;

namespace WorkBoard.Services.Abstraction.Services;

public interface ICardService
{
    Task<IReadOnlyList<CardDto>> GetCardsByBoardAsync(
        Guid boardId,
        CancellationToken cancellationToken = default);

    Task<CardDto> CreateCardAsync(
        Guid sectionId,
        CreateCardRequest request,
        CancellationToken cancellationToken = default);

    Task MoveCardAsync(
        Guid boardId,
        Guid cardId,
        MoveCardRequest request,
        CancellationToken cancellationToken = default);

    Task UpdateCardTitleAsync(
        Guid boardId,
        Guid cardId,
        UpdateCardTitleRequest request,
        CancellationToken cancellationToken = default);

    Task UpdateCardDescriptionAsync(
        Guid boardId,
        Guid cardId,
        UpdateCardDescriptionRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteCardAsync(
        Guid boardId,
        Guid cardId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CardAssigneeDto>> GetCardAssigneesAsync(
        Guid cardId,
        CancellationToken cancellationToken = default);

    Task AddCardAssigneeAsync(
        Guid cardId,
        AddCardAssigneeRequest request,
        CancellationToken cancellationToken = default);
}

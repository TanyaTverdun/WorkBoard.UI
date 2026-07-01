using Refit;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Requests;
using WorkBoard.Services.Abstraction.Requestsж;

namespace WorkBoard.Services.Servises.Card;

internal interface ICardApi
{
    [Get("/api/boards/{boardId}/cards")]
    Task<IReadOnlyList<CardDto>> GetCardsByBoardAsync(
        Guid boardId,
        CancellationToken cancellationToken = default);

    [Post("/api/sections/{sectionId}/cards")]
    Task<CardDto> CreateCardAsync(
        Guid sectionId,
        [Body] CreateCardRequest request,
        CancellationToken cancellationToken = default);

    [Patch("/api/boards/{boardId}/cards/{cardId}/move")]
    Task MoveCardAsync(
        Guid boardId,
        Guid cardId,
        [Body] MoveCardRequest request,
        CancellationToken cancellationToken = default);

    [Put("/api/boards/{boardId}/cards/{cardId}/title")]
    Task UpdateCardTitleAsync(
        Guid boardId,
        Guid cardId,
        [Body] UpdateCardTitleRequest request,
        CancellationToken cancellationToken = default);

    [Put("/api/boards/{boardId}/cards/{cardId}/description")]
    Task UpdateCardDescriptionAsync(
        Guid boardId,
        Guid cardId,
        [Body] UpdateCardDescriptionRequest request,
        CancellationToken cancellationToken = default);

    [Delete("/api/boards/{boardId}/cards/{cardId}")]
    Task DeleteCardAsync(
        Guid boardId,
        Guid cardId,
        CancellationToken cancellationToken = default);

    [Get("/api/cards/{cardId}/assignees")]
    Task<IReadOnlyList<CardAssigneeDto>> GetCardAssigneesAsync(
        Guid cardId,
        CancellationToken cancellationToken = default);

    [Post("/api/cards/{cardId}/assignees")]
    Task AddCardAssigneeAsync(
        Guid cardId,
        [Body] AddCardAssigneeRequest request,
        CancellationToken cancellationToken = default);

    [Get("/api/cards/{cardId}/assignable-users")]
    Task<IReadOnlyList<UserSearchDto>> GetAssignableUsersAsync(
        Guid cardId,
        CancellationToken cancellationToken = default);
}

using Microsoft.Extensions.Logging;
using Refit;
using WorkBoard.Services.Abstraction.DTOs.Cards;
using WorkBoard.Services.Abstraction.DTOs.Users;
using WorkBoard.Services.Abstraction.Requests.Cards;
using WorkBoard.Services.Abstraction.Services;

namespace WorkBoard.Services.Servises.Card;

internal class CardService : ICardService
{
    private readonly ICardApi _cardApi;
    private readonly ILogger<CardService> _logger;

    public CardService(
        ICardApi cardApi,
        ILogger<CardService> logger)
    {
        _cardApi = cardApi;
        _logger = logger;
    }

    public async Task<IReadOnlyList<CardDto>> GetCardsByBoardAsync(
        Guid boardId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cardApi.GetCardsByBoardAsync(
                boardId,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while fetching cards " +
                "for board {BoardId}. Status: {StatusCode}",
                boardId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while fetching cards for board {BoardId}",
                boardId);
            throw;
        }
    }

    public async Task<CardDetailsDto> GetCardDetailsAsync(
        Guid sectionId,
        Guid cardId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cardApi.GetCardDetailsAsync(
                sectionId,
                cardId, 
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error getting card details for card {CardId}. " +
                "Status: {StatusCode}",
                cardId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "Error getting details for card {CardId}", 
                cardId);
            throw;
        }
    }

    public async Task<CardDto> CreateCardAsync(
        Guid sectionId,
        CreateCardRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cardApi.CreateCardAsync(
                sectionId,
                request,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while creating card '{Title}' " +
                "in section {SectionId}. Status: {StatusCode}",
                request.Title,
                sectionId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while creating card '{Title}' " +
                "in section {SectionId}",
                request.Title,
                sectionId);
            throw;
        }
    }

    public async Task MoveCardAsync(
        Guid boardId,
        Guid cardId,
        MoveCardRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _cardApi.MoveCardAsync(
                boardId,
                cardId,
                request,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while moving card {CardId} " +
                "to section {SectionId} on board {BoardId}. Status: {StatusCode}",
                cardId,
                request.NewSectionId,
                boardId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while moving card {CardId} " +
                "to section {SectionId} on board {BoardId}",
                cardId,
                request.NewSectionId,
                boardId);
            throw;
        }
    }

    public async Task UpdateCardTitleAsync(
        Guid boardId,
        Guid cardId,
        UpdateCardTitleRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _cardApi.UpdateCardTitleAsync(
                boardId, 
                cardId, 
                request, 
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while updating title for card {CardId} " +
                "on board {BoardId}. Status: {StatusCode}",
                cardId, boardId, apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating title for card " +
                "{CardId} on board {BoardId}", cardId, boardId);
            throw;
        }
    }

    public async Task UpdateCardDescriptionAsync(
        Guid boardId,
        Guid cardId,
        UpdateCardDescriptionRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _cardApi.UpdateCardDescriptionAsync(
                boardId, 
                cardId, 
                request, 
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while updating description for card {CardId} " +
                "on board {BoardId}. Status: {StatusCode}",
                cardId, boardId, apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating description " +
                "for card {CardId} on board {BoardId}", cardId, boardId);
            throw;
        }
    }

    public async Task DeleteCardAsync(
        Guid boardId,
        Guid cardId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _cardApi.DeleteCardAsync(
                boardId, 
                cardId, 
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while deleting card {CardId} " +
                "on board {BoardId}. Status: {StatusCode}",
                cardId,
                boardId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while deleting card {CardId} " +
                "on board {BoardId}",
                cardId,
                boardId);
            throw;
        }
    }

    public async Task<IReadOnlyList<CardAssigneeDto>> GetCardAssigneesAsync(
        Guid cardId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cardApi.GetCardAssigneesAsync(
                cardId, 
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while fetching assignees " +
                "for card {CardId}. Status: {StatusCode}",
                cardId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while fetching assignees for card {CardId}",
                cardId);
            throw;
        }
    }

    public async Task AddCardAssigneeAsync(
        Guid cardId,
        AddCardAssigneeRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _cardApi.AddCardAssigneeAsync(
                cardId, 
                request, 
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while assigning user {UserId} " +
                "to card {CardId}. Status: {StatusCode}",
                request.UserId,
                cardId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while assigning user {UserId} to card {CardId}",
                request.UserId,
                cardId);
            throw;
        }
    }

    public async Task<IReadOnlyList<UserSearchDto>> GetAssignableUsersAsync(
        Guid cardId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cardApi.GetAssignableUsersAsync(
                cardId, 
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while fetching assignable users " +
                "for card {CardId}. Status: {StatusCode}",
                cardId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while fetching assignable users " +
                "for card {CardId}",
                cardId);
            throw;
        }
    }

    public async Task RemoveAssigneeAsync(
        Guid cardId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _cardApi.RemoveAssigneeAsync(
                cardId,
                userId,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while removing assignee {UserId} " +
                "from card {CardId}. Status: {StatusCode}",
                userId,
                cardId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while removing assignee {UserId} " +
                "from card {CardId}",
                userId,
                cardId);
            throw;
        }
    }

    public async Task UpdateCardDueDateAsync(
        Guid boardId,
        Guid cardId,
        UpdateCardDueDateRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _cardApi.UpdateCardDueDateAsync(
                boardId,
                cardId,
                request,
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while updating due date for card {CardId} " +
                "on board {BoardId}. Status: {StatusCode}",
                cardId,
                boardId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while updating due date for card {CardId} " +
                "on board {BoardId}",
                cardId,
                boardId);
            throw;
        }
    }
}

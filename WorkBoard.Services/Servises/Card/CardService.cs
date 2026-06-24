using Microsoft.Extensions.Logging;
using Refit;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Requests;
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
}

using Refit;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Requests;

namespace WorkBoard.Services.Servises.Card;

internal interface ICardApi
{
    [Post("/api/sections/{sectionId}/cards")]
    Task<CardDto> CreateCardAsync(
        Guid sectionId,
        [Body] CreateCardRequest request,
        CancellationToken cancellationToken = default);
}

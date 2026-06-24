using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Requests;

namespace WorkBoard.Services.Abstraction.Services;

public interface ICardService
{
    Task<CardDto> CreateCardAsync(
        Guid sectionId,
        CreateCardRequest request,
        CancellationToken cancellationToken = default);
}

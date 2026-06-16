using Refit;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Requests;

namespace WorkBoard.Services.Servises.Section;

internal interface ISectionApi
{
    [Get("/api/boards/{boardId}/sections")]
    Task<IReadOnlyList<SectionDto>> GetSectionsByBoardAsync(
        Guid boardId,
        CancellationToken cancellationToken = default);

    [Post("/api/boards/{boardId}/sections")]
    Task<Guid> CreateSectionAsync(
        Guid boardId,
        [Body] CreateSectionRequest request,
        CancellationToken cancellationToken = default);
}

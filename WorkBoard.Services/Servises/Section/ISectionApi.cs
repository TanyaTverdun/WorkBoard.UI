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

    [Put("/api/boards/{boardId}/sections/{sectionId}/name")]
    Task RenameSectionAsync(
        Guid boardId, 
        Guid sectionId, 
        [Body] UpdateSectionNameRequest request, 
        CancellationToken cancellationToken = default);

    [Delete("/api/boards/{boardId}/sections/{sectionId}")]
    Task DeleteSectionAsync(
        Guid boardId, 
        Guid sectionId, 
        CancellationToken cancellationToken = default);

    [Put("/api/boards/{boardId}/sections/{sectionId}/position")]
    Task MoveSectionAsync(
        Guid boardId,
        Guid sectionId,
        MoveSectionRequest request,
        CancellationToken cancellationToken = default);
}

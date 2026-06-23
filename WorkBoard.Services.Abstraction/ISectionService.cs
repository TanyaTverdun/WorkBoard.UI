using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Requests;

namespace WorkBoard.Services.Abstraction;

public interface ISectionService
{
    Task<IReadOnlyList<SectionDto>> GetSectionsByBoardAsync(
        Guid boardId, 
        CancellationToken cancellationToken = default);

    Task<Guid> CreateSectionAsync(
        Guid boardId, 
        CreateSectionRequest request, 
        CancellationToken cancellationToken = default);

    Task RenameSectionAsync(
        Guid boardId, 
        Guid sectionId, 
        UpdateSectionNameRequest request, 
        CancellationToken cancellationToken = default);

    Task DeleteSectionAsync(
        Guid boardId, 
        Guid sectionId, 
        CancellationToken cancellationToken = default);

    Task MoveSectionAsync(
        Guid boardId,
        Guid sectionId,
        MoveSectionRequest request,
        CancellationToken cancellationToken = default);
}

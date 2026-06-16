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
}

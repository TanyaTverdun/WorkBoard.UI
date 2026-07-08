using WorkBoard.Services.Abstraction.DTOs;

namespace WorkBoard.Services.Abstraction.Services;

public interface ICommentService
{
    Task<IReadOnlyList<CommentDto>> GetCommentsByCardAsync(
        Guid cardId,
        CancellationToken cancellationToken = default);
}

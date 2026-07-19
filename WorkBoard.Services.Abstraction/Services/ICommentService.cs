using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Requests;

namespace WorkBoard.Services.Abstraction.Services;

public interface ICommentService
{
    Task<CommentDto> CreateCommentAsync(
        Guid cardId,
        CreateCommentRequest request,
        CancellationToken cancellationToken = default);
}

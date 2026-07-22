using WorkBoard.Services.Abstraction.DTOs.Comments;
using WorkBoard.Services.Abstraction.Requests.Comments;

namespace WorkBoard.Services.Abstraction.Services;

public interface ICommentService
{
    Task<CommentDto> CreateCommentAsync(
        Guid cardId,
        CreateCommentRequest request,
        CancellationToken cancellationToken = default);
}

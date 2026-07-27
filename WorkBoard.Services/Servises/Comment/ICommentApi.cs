using Refit;
using WorkBoard.Services.Abstraction.DTOs.Comments;
using WorkBoard.Services.Abstraction.Requests.Comments;

namespace WorkBoard.Services.Servises.Comment;

internal interface ICommentApi
{
    [Post("/api/cards/{cardId}/comments")]
    Task<CommentDto> CreateCommentAsync(
        Guid cardId,
        [Body] CreateCommentRequest request,
        CancellationToken cancellationToken = default);
}

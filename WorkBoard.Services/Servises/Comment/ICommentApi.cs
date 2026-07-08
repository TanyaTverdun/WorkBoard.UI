using Refit;
using WorkBoard.Services.Abstraction.DTOs;

namespace WorkBoard.Services.Servises.Comment;

internal interface ICommentApi
{
    [Get("/api/cards/{cardId}/comments")]
    Task<IReadOnlyList<CommentDto>> GetCommentsByCardAsync(
        Guid cardId,
        CancellationToken cancellationToken = default);
}

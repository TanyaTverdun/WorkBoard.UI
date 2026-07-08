using Refit;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Requests;

namespace WorkBoard.Services.Servises.Comment;

internal interface ICommentApi
{
    [Get("/api/cards/{cardId}/comments")]
    Task<IReadOnlyList<CommentDto>> GetCommentsByCardAsync(
        Guid cardId,
        CancellationToken cancellationToken = default);

    [Post("/api/cards/{cardId}/comments")]
    Task<CommentDto> CreateCommentAsync(
        Guid cardId,
        [Body] CreateCommentRequest request,
        CancellationToken cancellationToken = default);
}

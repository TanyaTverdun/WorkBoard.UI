using Microsoft.Extensions.Logging;
using Refit;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Requests;
using WorkBoard.Services.Abstraction.Services;

namespace WorkBoard.Services.Servises.Comment;

internal class CommentService : ICommentService
{
    private readonly ICommentApi _commentApi;
    private readonly ILogger<CommentService> _logger;

    public CommentService(
        ICommentApi commentApi,
        ILogger<CommentService> logger)
    {
        _commentApi = commentApi;
        _logger = logger;
    }

    public async Task<IReadOnlyList<CommentDto>> GetCommentsByCardAsync(
        Guid cardId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _commentApi.GetCommentsByCardAsync(
                cardId, 
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while fetching comments for card {CardId}. " +
                "Status: {StatusCode}",
                cardId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while fetching comments for card {CardId}",
                cardId);
            throw;
        }
    }

    public async Task<CommentDto> CreateCommentAsync(
        Guid cardId,
        CreateCommentRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _commentApi.CreateCommentAsync(
                cardId, 
                request, 
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx,
                "API error occurred while creating a comment for card {CardId}. " +
                "Status: {StatusCode}",
                cardId,
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while creating a comment for card {CardId}",
                cardId);
            throw;
        }
    }
}

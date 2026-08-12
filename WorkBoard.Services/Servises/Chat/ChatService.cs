using Microsoft.Extensions.Logging;
using Refit;
using WorkBoard.Services.Abstraction.DTOs.Chat;
using WorkBoard.Services.Abstraction.Requests.Chat;
using WorkBoard.Services.Abstraction.Services;

namespace WorkBoard.Services.Servises.Chat;

internal class ChatService : IChatService
{
    private readonly IChatApi _chatApi;
    private readonly ILogger<ChatService> _logger;

    public ChatService(IChatApi chatApi, ILogger<ChatService> logger)
    {
        _chatApi = chatApi;
        _logger = logger;
    }

    public async Task<ChatResponseDto> AskAiAsync(
        Guid workspaceId,
        ChatRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _chatApi.AskAiAsync(
                workspaceId, 
                request, 
                cancellationToken);
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(
                apiEx, 
                "API error occurred while asking AI for workspace " +
                "{WorkspaceId}. Status: {StatusCode}", 
                workspaceId, 
                apiEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "Error occurred while asking AI for workspace {WorkspaceId}", 
                workspaceId);
            throw;
        }
    }
}

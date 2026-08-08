using Refit;
using WorkBoard.Services.Abstraction.DTOs.Chat;
using WorkBoard.Services.Abstraction.Requests.Chat;

namespace WorkBoard.Services.Servises.Chat;

internal interface IChatApi
{
    [Post("/api/chat/ask")]
    Task<ChatResponseDto> AskAiAsync(
        [Header("X-Workspace-Id")] Guid workspaceId,
        [Body] ChatRequestDto request,
        CancellationToken cancellationToken = default);
}

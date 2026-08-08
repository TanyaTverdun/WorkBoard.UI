using WorkBoard.Services.Abstraction.DTOs.Chat;
using WorkBoard.Services.Abstraction.Requests.Chat;

namespace WorkBoard.Services.Abstraction.Services;

public interface IChatService
{
    Task<ChatResponseDto> AskAiAsync(
        Guid workspaceId,
        ChatRequestDto request,
        CancellationToken cancellationToken = default);
}

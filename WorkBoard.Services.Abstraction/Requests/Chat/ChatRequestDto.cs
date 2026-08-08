using WorkBoard.Services.Abstraction.DTOs.Chat;

namespace WorkBoard.Services.Abstraction.Requests.Chat;

public class ChatRequestDto
{
    public List<ChatMessageDto> Messages { get; set; } = new();
}

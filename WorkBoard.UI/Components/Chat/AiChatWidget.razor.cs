using Microsoft.AspNetCore.Components;
using WorkBoard.Domain.Enums;
using WorkBoard.Services.Abstraction.DTOs.Chat;
using WorkBoard.Services.Abstraction.Requests.Chat;
using WorkBoard.Services.Abstraction.Services;
using WorkBoard.Services.StateProviders;

namespace WorkBoard.UI.Components.Chat
{
    public partial class AiChatWidget : IDisposable
    {
        [Inject] 
        private IChatService ChatService { get; set; } = null!;
        [Inject] 
        private WorkspaceStateProvider WorkspaceState { get; set; } = null!;

        private bool _isOpen = false;
        private bool _isLoading = false;
        private string _currentInput = string.Empty;
        private List<ChatMessageDto> _messages = new();

        public const string ChatRoleUser = "User";
        public const string ChatRoleAssistant = "Assistant";
        public const string ErrorMessageContent = "Sorry, an error occurred while connecting to the AI.";

        protected override void OnInitialized()
        {
            WorkspaceState.OnWorkspaceChanged += HandleWorkspaceChanged;
        }

        private void HandleWorkspaceChanged(
            Guid? workspaceId, 
            WorkspaceRole? role)
        {
            _messages.Clear();
            _isOpen = false;
            InvokeAsync(StateHasChanged);
        }

        private void ToggleChat() => _isOpen = !_isOpen;

        private async Task SendSuggestion(string text)
        {
            _currentInput = text;
            await SendMessageAsync();
        }

        private async Task SendMessageAsync()
        {
            if (string.IsNullOrWhiteSpace(_currentInput) ||
                _isLoading ||
                WorkspaceState.SelectedWorkspaceId == null)
            {
                return;
            }

            var userMessage = new ChatMessageDto 
            { 
                Role = ChatRoleUser, 
                Content = _currentInput 
            
            };
            _messages.Add(userMessage);

            _currentInput = string.Empty;
            _isLoading = true;
            StateHasChanged();

            try
            {
                var request = new ChatRequestDto
                {
                    Messages = _messages.ToList()
                };

                var response = await ChatService.AskAiAsync(
                    WorkspaceState.SelectedWorkspaceId.Value,
                    request);

                _messages.Add(new ChatMessageDto 
                { 
                    Role = ChatRoleAssistant, 
                    Content = response.Answer 
                });
            }
            catch (Exception)
            {
                _messages.Add(new ChatMessageDto 
                {
                    Role = ChatRoleAssistant, 
                    Content = ErrorMessageContent
                });
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        public void Dispose()
        {
            WorkspaceState.OnWorkspaceChanged -= HandleWorkspaceChanged;
        }
    }
}

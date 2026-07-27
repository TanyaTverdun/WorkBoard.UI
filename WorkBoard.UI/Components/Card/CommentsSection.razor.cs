using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using WorkBoard.Services.Abstraction.DTOs.Comments;
using WorkBoard.Services.Abstraction.DTOs.Users;
using WorkBoard.Services.Abstraction.Hubs;
using WorkBoard.Services.Abstraction.Requests.Comments;
using WorkBoard.Services.Abstraction.Services;
using WorkBoard.Services.Abstraction.StateProviders;

namespace WorkBoard.UI.Components.Card;

public partial class CommentsSection : ComponentBase, IDisposable
{
    [Parameter]
    public Guid CardId { get; set; }

    [Parameter]
    public Guid CurrentUserId { get; set; }

    [Parameter]
    public EventCallback<int> CommentsCountChanged { get; set; }

    [Parameter] 
    public List<CommentDto> Comments { get; set; } = new();

    [Parameter]
    public bool IsObserver { get; set; }

    [Inject]
    private ICurrentUserProvider CurrentUserProvider { get; set; } = default!;

    [Inject]
    private ICommentService CommentService { get; set; } = default!;

    [Inject]
    private IBoardHubService BoardHubService { get; set; } = default!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    private ElementReference _scrollContainer;
    private IJSObjectReference? _jsModule;

    private List<CommentDto> _comments = new();
    private string _newComment = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        BoardHubService.OnCommentAdded += HandleNewComment;
        CurrentUserProvider.OnProfileChanged += HandleProfileChanged;
        BoardHubService.OnUserAvatarUpdated += HandleUserAvatarUpdated;
    }

    protected override void OnParametersSet()
    {
        if (Comments != null)
        {
            _comments = Comments.ToList();
            StateHasChanged();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                "import",
                "./Components/Card/CommentsSection.razor.js");
        }
    }

    private async Task AddCommentAsync()
    {
        if (IsObserver)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_newComment))
        {
            return;
        }

        try
        {
            var request = new CreateCommentRequest 
            { 
                Text = _newComment.Trim() 
            };

            var dto = await CommentService.CreateCommentAsync(
                CardId,
                request);

            dto.UserFullName = !string.IsNullOrWhiteSpace(dto.UserFullName) ? 
                dto.UserFullName : "Unknown User";

            dto.Initials = !string.IsNullOrWhiteSpace(dto.Initials) ? 
                dto.Initials : "UU";

            _comments.Add(dto);
            Comments.Add(dto);
            _newComment = string.Empty;

            await NotifyCountChangedAsync();
            StateHasChanged();

            await Task.Delay(50);
            await ScrollToBottomAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding comment: {ex.Message}");
            Snackbar.Add("Failed to add comment.", Severity.Error);
        }
    }

    private async Task ScrollToBottomAsync()
    {
        try
        {
            if (_jsModule != null)
            {
                await _jsModule.InvokeVoidAsync("scrollToBottom", _scrollContainer);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Scroll error: {ex.Message}");
        }
    }

    private void HandleNewComment(CommentDto newComment)
    {
        if (newComment.CardId == CardId && !_comments.Any(
            c => c.Id == newComment.Id))
        {
            newComment.UserFullName = !string.IsNullOrWhiteSpace(newComment.UserFullName) ? 
                newComment.UserFullName : "Unknown User";

            newComment.Initials = !string.IsNullOrWhiteSpace(newComment.Initials) ? 
                newComment.Initials : "UU";

            _comments.Add(newComment);

            Comments.Add(newComment);

            InvokeAsync(async () =>
            {
                await NotifyCountChangedAsync();
                StateHasChanged();

                await Task.Delay(50);
                await ScrollToBottomAsync();
            });
        }
    }

    private async Task NotifyCountChangedAsync()
    {
        if (CommentsCountChanged.HasDelegate)
        {
            await CommentsCountChanged.InvokeAsync(_comments.Count);
        }
    }

    private void HandleProfileChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    private void HandleUserAvatarUpdated(UserAvatarUpdatedDto data)
    {
        bool changed = false;

        for (int i = 0; i < _comments.Count; i++)
        {
            if (_comments[i].UserId == data.UserId)
            {
                _comments[i].UserAvatarColor = data.AvatarColor;
                _comments[i].UserAvatarUrl = data.AvatarUrl;

                changed = true;
            }
        }

        for (int i = 0; i < Comments.Count; i++)
        {
            if (Comments[i].UserId == data.UserId)
            {
                Comments[i].UserAvatarColor = data.AvatarColor;
                Comments[i].UserAvatarUrl = data.AvatarUrl;

                changed = true;
            }
        }

        if (changed)
        {
            InvokeAsync(StateHasChanged);
        }
    }

    public void Dispose()
    {
        BoardHubService.OnCommentAdded -= HandleNewComment;
        CurrentUserProvider.OnProfileChanged -= HandleProfileChanged;
        BoardHubService.OnUserAvatarUpdated -= HandleUserAvatarUpdated;

        _ = _jsModule?.DisposeAsync();
    }
}

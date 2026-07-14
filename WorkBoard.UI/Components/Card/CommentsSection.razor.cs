using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Hubs;
using WorkBoard.Services.Abstraction.Requests;
using WorkBoard.Services.Abstraction.Services;

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
        if (string.IsNullOrWhiteSpace(_newComment)) return;

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

    public void Dispose()
    {
        BoardHubService.OnCommentAdded -= HandleNewComment;

        _ = _jsModule?.DisposeAsync();
    }
}

using Microsoft.AspNetCore.Components;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Hubs;
using WorkBoard.Services.Abstraction.Requests;
using WorkBoard.Services.Abstraction.Services;

namespace WorkBoard.UI.Components.Card;

public partial class CardAssigneesSection : ComponentBase, IDisposable
{
    [Parameter]
    public Guid CardId { get; set; }

    [Parameter]
    public List<CardAssigneeDto> Assignees { get; set; } = new();

    [Parameter]
    public EventCallback<List<CardAssigneeDto>> AssigneesChanged { get; set; }

    [Parameter]
    public List<UserSearchDto> AssignableUsers { get; set; } = new();

    [Inject]
    private ICardService CardService { get; set; } = default!;

    [Inject]
    private IBoardHubService BoardHubService { get; set; } = default!;

    private bool _isPopoverOpen = false;
    private string _searchText = string.Empty;
    private List<CardAssigneeDto> _assignees = new();

    private IEnumerable<UserSearchDto> FilteredAssignableUsers =>
        string.IsNullOrWhiteSpace(_searchText)
            ? AssignableUsers
            : AssignableUsers.Where(u =>
                u.FullName.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains(_searchText, StringComparison.OrdinalIgnoreCase));

    protected override void OnInitialized()
    {
        BoardHubService.OnAssigneeAdded += HandleAssigneeAdded;
        BoardHubService.OnAssigneeRemoved += HandleAssigneeRemoved;
    }

    protected override void OnParametersSet()
    {
        if (Assignees != null)
        {
            _assignees = Assignees.ToList();
        }
    }

    private async Task AddAssigneeAsync(UserSearchDto user)
    {
        try
        {
            var request = new AddCardAssigneeRequest(user.UserId);
            await CardService.AddCardAssigneeAsync(CardId, request);

            var newAssignee = new CardAssigneeDto(
                user.UserId,
                user.FullName,
                user.Email,
                user.AvatarUrl,
                user.Initials ?? "Un");

            _assignees.Add(newAssignee);
            Assignees.Add(newAssignee);

            await AssigneesChanged.InvokeAsync(Assignees);
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding assignee: {ex.Message}");
        }
    }

    private async Task RemoveAssigneeAsync(CardAssigneeDto assignee)
    {
        try
        {
            await CardService.RemoveAssigneeAsync(CardId, assignee.UserId);

            _assignees.RemoveAll(a => a.UserId == assignee.UserId);
            Assignees.RemoveAll(a => a.UserId == assignee.UserId);

            await AssigneesChanged.InvokeAsync(Assignees);
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing assignee: {ex.Message}");
        }
    }

    private void HandleAssigneeAdded(AssigneeAddDto data)
    {
        if (CardId == data.CardId && !_assignees.Any(a => a.UserId == data.Assignee.UserId))
        {
            _assignees.Add(data.Assignee);
            Assignees.Add(data.Assignee);

            InvokeAsync(async () =>
            {
                await AssigneesChanged.InvokeAsync(Assignees);
                StateHasChanged();
            });
        }
    }

    private void HandleAssigneeRemoved(AssigneeRemoveDto data)
    {
        if (CardId == data.CardId)
        {
            var removedCount = _assignees.RemoveAll(a => a.UserId == data.UserId);

            if (removedCount > 0)
            {
                Assignees.RemoveAll(a => a.UserId == data.UserId);

                InvokeAsync(async () =>
                {
                    await AssigneesChanged.InvokeAsync(Assignees);
                    StateHasChanged();
                });
            }
        }
    }

    public void Dispose()
    {
        BoardHubService.OnAssigneeAdded -= HandleAssigneeAdded;
        BoardHubService.OnAssigneeRemoved -= HandleAssigneeRemoved;
    }
}

using Microsoft.AspNetCore.Components;
using WorkBoard.Services.Abstraction.DTOs.Cards;
using WorkBoard.Services.Abstraction.DTOs.Users;
using WorkBoard.Services.Abstraction.Hubs;
using WorkBoard.Services.Abstraction.Requests.Cards;
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

    [Parameter]
    public bool IsObserver { get; set; }

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
        BoardHubService.OnUserAvatarUpdated += HandleUserAvatarUpdated;
    }

    protected override void OnParametersSet()
    {
        if (Assignees != null)
        {
            _assignees = Assignees.DistinctBy(a => a.UserId).ToList();
        }
    }

    public async Task ToggleAssigneeAsync(UserSearchDto user)
    {
        if (IsObserver)
        {
            return;
        }

        var existingAssignee = _assignees
            .FirstOrDefault(a => a.UserId == user.UserId);

        if (existingAssignee != null)
        {
            await RemoveAssigneeAsync(existingAssignee);
        }
        else
        {
            await AddAssigneeAsync(user);
        }
    }

    private async Task AddAssigneeAsync(UserSearchDto user)
    {
        if (IsObserver)
        {
            return;
        }

        try
        {
            var request = new AddCardAssigneeRequest(user.UserId);

            await CardService.AddCardAssigneeAsync(CardId, request);

            if (!_assignees.Any(a => a.UserId == user.UserId))
            {
                var newAssignee = new CardAssigneeDto(
                    user.UserId,
                    user.FullName,
                    user.Email,
                    user.AvatarUrl,
                    user.AvatarColor,
                    user.Initials ?? "Un");

                _assignees.Add(newAssignee);
                Assignees.Add(newAssignee);

                await AssigneesChanged.InvokeAsync(_assignees);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding assignee: {ex.Message}");
        }
        finally
        {
            StateHasChanged();
        }
    }

    private async Task RemoveAssigneeAsync(CardAssigneeDto assignee)
    {
        if (IsObserver)
        {
            return;
        }

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

    private void HandleUserAvatarUpdated(UserAvatarUpdatedDto data)
    {
        bool changed = false;

        for (int i = 0; i < _assignees.Count; i++)
        {
            if (_assignees[i].UserId == data.UserId)
            {
                _assignees[i] = _assignees[i] with
                {
                    AvatarColor = data.AvatarColor ?? _assignees[i].AvatarColor,
                    AvatarUrl = data.AvatarUrl
                };
                changed = true;
            }
        }

        for (int i = 0; i < Assignees.Count; i++)
        {
            if (Assignees[i].UserId == data.UserId)
            {
                Assignees[i] = Assignees[i] with
                {
                    AvatarColor = data.AvatarColor ?? Assignees[i].AvatarColor,
                    AvatarUrl = data.AvatarUrl
                };
                changed = true;
            }
        }

        for (int i = 0; i < AssignableUsers.Count; i++)
        {
            if (AssignableUsers[i].UserId == data.UserId)
            {
                AssignableUsers[i].AvatarColor = data.AvatarColor;
                AssignableUsers[i].AvatarUrl = data.AvatarUrl;

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
        BoardHubService.OnAssigneeAdded -= HandleAssigneeAdded;
        BoardHubService.OnAssigneeRemoved -= HandleAssigneeRemoved;
        BoardHubService.OnUserAvatarUpdated -= HandleUserAvatarUpdated;
    }
}

using Microsoft.AspNetCore.Components;
using WorkBoard.Services.Abstraction.DTOs;
using WorkBoard.Services.Abstraction.Hubs;

namespace WorkBoard.UI.Components.Card;

public partial class ActivityLogSection : ComponentBase, IDisposable
{
    [Parameter]
    public Guid CardId { get; set; }

    [Parameter]
    public List<ActivityLogDto> ActivityLogs { get; set; } = new();

    [Parameter]
    public EventCallback<List<ActivityLogDto>> ActivityLogsChanged { get; set; }

    [Inject]
    private IBoardHubService BoardHubService { get; set; } = default!;

    private List<ActivityLogDto> _activityLogs = new();

    protected override void OnInitialized()
    {
        BoardHubService.OnActivityLogAdded += HandleActivityLogAdded;
    }

    protected override void OnParametersSet()
    {
        if (ActivityLogs != null)
        {
            _activityLogs = ActivityLogs.ToList();
        }
    }

    private void HandleActivityLogAdded(ActivityLogDto log)
    {
        if (log.CardId == CardId)
        {
            _activityLogs.Insert(0, log);
            ActivityLogs.Insert(0, log);

            InvokeAsync(async () =>
            {
                await ActivityLogsChanged.InvokeAsync(ActivityLogs);
                StateHasChanged();
            });
        }
    }

    public void Dispose()
    {
        BoardHubService.OnActivityLogAdded -= HandleActivityLogAdded;
    }
}

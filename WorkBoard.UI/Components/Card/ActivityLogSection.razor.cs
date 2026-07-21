using Microsoft.AspNetCore.Components;
using WorkBoard.Services.Abstraction.DTOs;

namespace WorkBoard.UI.Components.Card;

public partial class ActivityLogSection : ComponentBase
{
    [Parameter]
    public List<ActivityLogDto> ActivityLogs { get; set; } = new();

    private List<ActivityLogDto> _activityLogs = new();

    protected override void OnParametersSet()
    {
        if (ActivityLogs != null)
        {
            _activityLogs = ActivityLogs.ToList();
        }
    }
}

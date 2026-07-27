using Microsoft.AspNetCore.Components;
using WorkBoard.Services.Abstraction.DTOs.ActivityLogs;
using WorkBoard.Services.Abstraction.DTOs.Users;
using WorkBoard.Services.Abstraction.Hubs;

namespace WorkBoard.UI.Components.Card;

public partial class ActivityLogSection : ComponentBase
{
    [Parameter]
    public List<ActivityLogDto> ActivityLogs { get; set; } = new();

    [Inject]
    private IBoardHubService BoardHubService { get; set; } = default!;

    protected override void OnInitialized()
    {
        BoardHubService.OnUserAvatarUpdated += HandleUserAvatarUpdated;
    }

    private void HandleUserAvatarUpdated(UserAvatarUpdatedDto data)
    {
        bool changed = false;

        for (int i = 0; i < ActivityLogs.Count; i++)
        {
            if (ActivityLogs[i].UserId == data.UserId)
            {
                ActivityLogs[i].AvatarColor = data.AvatarColor;
                ActivityLogs[i].AvatarUrl = data.AvatarUrl;

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
        BoardHubService.OnUserAvatarUpdated -= HandleUserAvatarUpdated;
    }
}

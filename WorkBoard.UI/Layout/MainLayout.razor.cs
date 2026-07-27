using Microsoft.AspNetCore.Components;
using MudBlazor;
using WorkBoard.Services.Abstraction.StateProviders;

namespace WorkBoard.UI.Layout;

public partial class MainLayout
{
    [Inject]
    ICurrentUserProvider CurrentUserProvider { get; set; } = default!;

    private MudTheme _customTheme = new MudTheme()
    {
    };

    private bool _isSidebarOpen = true;

    private void ToggleSidebar()
    {
        _isSidebarOpen = !_isSidebarOpen;
    }

    protected override async Task OnInitializedAsync()
    {
        await CurrentUserProvider.LoadProfileAsync();
    }
}

using MudBlazor;

namespace WorkBoard.UI.Layout;

public partial class MainLayout
{
    private MudTheme _customTheme = new MudTheme()
    {
    };

    private bool _isSidebarOpen = true;

    private void ToggleSidebar()
    {
        _isSidebarOpen = !_isSidebarOpen;
    }
}

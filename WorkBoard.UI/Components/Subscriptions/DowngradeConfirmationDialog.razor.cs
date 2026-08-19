using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace WorkBoard.UI.Components.Subscriptions;

public partial class DowngradeConfirmationDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;

    private void Cancel() => MudDialog.Cancel();

    private void Confirm() => MudDialog.Close(DialogResult.Ok(true));
}
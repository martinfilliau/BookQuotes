using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BookQuotes.App.Components;

public partial class AlertDialogComponent : ComponentBase
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; }
    [Parameter] public string Message { get; set; }

    private void Submit() => MudDialog.Close(DialogResult.Ok(true));

    private void Cancel() => MudDialog.Cancel();
}
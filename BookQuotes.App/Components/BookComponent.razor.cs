using System.Text;
using BookQuotes.App.Services;
using BookQuotes.Application.Commands;
using BookQuotes.Domain.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace BookQuotes.App.Components;

public partial class BookComponent : ComponentBase
{
    [Parameter] public required Book Book { get; set; }

    [Inject] IExportBook ExportBook { get; set; } = null!;
    [Inject] FileDownloaderService FileDownloaderService { get; set; } = null!;
    [Inject] IJSRuntime JSRuntime { get; set; } = null!;
    [Inject] ISnackbar Snackbar { get; set; } = null!;
    [Inject] IDialogService DialogService { get; set; } = null!;

    private TableOfContentsItem? RootToc => Book?.TableOfContents?.Root;

    protected override void OnParametersSet()
    {
        Book?.UpdateQuotesWithTableOfContents();
    }

    private async Task ExportQuotesMarkdown()
    {
        string? error = null;
        var success = false;

        try
        {
            var markdown = ExportBook.ExportToMarkdown(Book);
            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(markdown));
            await FileDownloaderService.DownloadFileAsync($"{Book.Title}.md", memoryStream, "text/plain");           
            success = true;
        }
        catch (Exception e)
        {
            error = e.Message;
        }

        if (success)
        {
            Snackbar.Add("File successfully downloaded", Severity.Success);
        }

        if (!string.IsNullOrWhiteSpace(error))
        {
            await DialogService.ShowMessageBox("Error: unable to export", error);
        }
    }

    private async Task CopyToClipboard()
    {
        string? error = null;
        var success = false;
        
        try
        {
            var markdown = ExportBook.ExportToMarkdown(Book);
            await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", markdown);
            success = true;
        }
        catch (Exception e)
        {
            error = e.Message;
        }

        if (success)
        {
            Snackbar.Add("Markdown copied to clipboard", Severity.Success);
        }
        
        if (!string.IsNullOrWhiteSpace(error))
        {
            await DialogService.ShowMessageBox("Error: unable to copy", error);
        }
    }
}
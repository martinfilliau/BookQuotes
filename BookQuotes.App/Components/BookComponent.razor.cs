using System.Text;
using BookQuotes.App.Services;
using BookQuotes.Application.Commands;
using BookQuotes.Domain.Entities;
using Microsoft.AspNetCore.Components;

namespace BookQuotes.App.Components;

public partial class BookComponent : ComponentBase
{
    [Parameter] public required Book Book { get; set; }

    [Inject] IExportBook ExportBook { get; set; } = null!;
    [Inject] FileDownloaderService FileDownloaderService { get; set; } = null!;

    private TableOfContentsItem? RootToc => Book?.TableOfContents?.Root;

    protected override void OnParametersSet()
    {
        Book?.UpdateQuotesWithTableOfContents();
    }

    private async Task ExportQuotesMarkdown()
    {
        var markdown = ExportBook.ExportToMarkdown(Book);
        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(markdown));
        await FileDownloaderService.DownloadFileAsync($"{Book.Title}.md", memoryStream, "text/plain");
    }
}
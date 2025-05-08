using System.Text;
using BookQuotes.App.Services;
using BookQuotes.Application.Commands;
using BookQuotes.Domain.Entities;
using Microsoft.AspNetCore.Components;

namespace BookQuotes.App.Components;

public partial class BookComponent : ComponentBase
{
    [Parameter] public Book Book { get; set; }

    [Inject] IExportQuotes ExportQuotes { get; set; }
    [Inject] FileDownloaderService FileDownloaderService { get; set; }
    
    List<string> TocItems =>
        Book?.TableOfContents == null
            ? []
            : Book.TableOfContents.GetTableOfContentsAsArray();

    private async Task ExportQuotesMarkdown()
    {
        var markdown = ExportQuotes.ExportQuotesToMarkdown(Book, Book.Quotes);
        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(markdown));
        await FileDownloaderService.DownloadFileAsync($"{Book.Title} - quotes.md", memoryStream, "text/plain");
    }
}
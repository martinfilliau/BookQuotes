using System.Text;
using BookQuotes.App.Services;
using BookQuotes.Application.Commands;
using BookQuotes.Domain.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace BookQuotes.App.Components;

public partial class UploadComponent : ComponentBase
{
    [Inject] IDialogService DialogService { get; set; }
    [Inject] IAnalyseEpubFile AnalyseEpubFile { get; set; }
    [Inject] IAnalyseQuotes AnalyseQuotes { get; set; }
    [Inject] IExportQuotes ExportQuotes { get; set; }
    [Inject] FileDownloaderService FileDownloaderService { get; set; }

    private const int MaxFileSize = 30000000; // 30 MB

    private IBrowserFile? _epub;
    private IBrowserFile? _xml;
    private bool _canStartAnalysis = false;

    private Book? _book;
    
    private bool CanExportQuotes => _book is not null && _book.Quotes.Count > 0;
    
    private async Task UploadFiles(IReadOnlyList<IBrowserFile> files)
    {
        //      --> check title matches
       
        // Append multiple files
        
        foreach (var file in files)
        {
            if (IsEpubFile(file))
            {
                _epub = file;
                _canStartAnalysis = true;
                break;
            }

            if (IsAnnotationsFile(file))
            {
                _xml = file;
                _canStartAnalysis = true;
                break;
            }

            await UnknownFile(file);
        }
    }

    private void Reset()
    {
        _canStartAnalysis = false;
        _epub = null;
        _xml = null;
        _book = null;
    }

    async Task StartAnalysis()
    {
        if (!_canStartAnalysis)
            return;
        
        if (_epub != null)
        {
            await using var stream = _epub.OpenReadStream(MaxFileSize);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
 
            var result = await AnalyseEpubFile.Analyse(ms);
            _book = result;
        }

        if (_xml != null)
        {
            using var reader = new StreamReader(_xml.OpenReadStream());
            var text = await reader.ReadToEndAsync();
            _book = AnalyseQuotes.Analyse(text);
        }
    }

    async Task ExportQuotesMarkdown()
    {
        if (_book == null)
            return;
        var markdown = ExportQuotes.ExportQuotesToMarkdown(_book, _book.Quotes);
        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(markdown));
        await FileDownloaderService.DownloadFileAsync($"{_book.Title} - quotes.md", memoryStream, "text/plain");
    }
    
    private static bool IsEpubFile(IBrowserFile file) => file.ContentType == "application/epub+zip" || file.Name.EndsWith(".epub");
    
    private static bool IsAnnotationsFile(IBrowserFile file) => file.Name.EndsWith(".xml") || file.Name.EndsWith(".annot");

    private async Task UnknownFile(IBrowserFile file)
    {
        await DialogService.ShowMessageBox("Alert",
            $"File {file.Name} is not an epub file nor an XML (annotations) file.");
    }
}
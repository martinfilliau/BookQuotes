using BookQuotes.Application.Commands;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace BookQuotes.App.Components;

public partial class UploadComponent : ComponentBase
{
    [Inject] IDialogService DialogService { get; set; }
    [Inject] IAnalyseEpubFile AnalyseEpubFile { get; set; }
    [Inject] IAnalyseQuotes AnalyseQuotes { get; set; }

    private const int MaxFileSize = 30000000; // 30 MB

    private IBrowserFile? _epub;
    private IBrowserFile? _xml;
    private bool _canStartAnalysis = false;

    private string output = "";
    
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
        }

        if (_xml != null)
        {
            using var reader = new StreamReader(_xml.OpenReadStream());
            var text = await reader.ReadToEndAsync();
            var quotes = AnalyseQuotes.Analyse(text);

            output = string.Join(',', quotes.Select(quote => quote.Comment));
        }
    }
    
    private static bool IsEpubFile(IBrowserFile file) => file.ContentType == "application/epub+zip" || file.Name.EndsWith(".epub");
    
    private static bool IsAnnotationsFile(IBrowserFile file) => file.Name.EndsWith(".xml");

    private async Task UnknownFile(IBrowserFile file)
    {
        var parameters = new DialogParameters<AlertDialogComponent>()
        {
            { d => d.Message, $"File {file.Name} is not an epub file nor an XML (annotations) file." }
        };
        await DialogService.ShowAsync<AlertDialogComponent>("Alert", parameters);
    }
}
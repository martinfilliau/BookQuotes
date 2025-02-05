using BookQuotes.Application.Commands;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using VersOne.Epub;

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

    private async Task HandleFile(IBrowserFile file)
    {
        if (IsEpubFile(file))
        {
            await using var stream = file.OpenReadStream(MaxFileSize);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            using var book = await EpubReader.OpenBookAsync(ms);
             
        }
        else if (IsAnnotationsFile(file))
        {
            
        }
    }

    async Task StartAnalysis()
    {
        if (!_canStartAnalysis)
            return;
        
        // XXX TODO check epub is not null
        
        await using var stream = _epub.OpenReadStream(MaxFileSize);
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);

        var result = await AnalyseEpubFile.Analyse(ms);
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
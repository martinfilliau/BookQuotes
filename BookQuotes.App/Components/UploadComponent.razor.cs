using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using VersOne.Epub;

namespace BookQuotes.App.Components;

public partial class UploadComponent : ComponentBase
{
    [Inject] IDialogService DialogService { get; set; }
    private string _title = string.Empty;

    private const int MaxFileSize = 30000000; // 30 MB
    
    private async Task UploadFiles(IReadOnlyList<IBrowserFile> files)
    {
        foreach (var file in files)
        {
            await HandleFile(file);
        }
    }

    private async Task HandleFile(IBrowserFile file)
    {
        if (file.ContentType == "application/epub+zip" || file.Name.EndsWith(".epub"))
        {
            await using var stream = file.OpenReadStream(MaxFileSize);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            using var book = await EpubReader.OpenBookAsync(ms);
             
        }
        else if (file.Name.EndsWith(".xml"))
        {
            
        }
        else
        {
            var parameters = new DialogParameters<AlertDialogComponent>()
            {
                { d => d.Message, "" }
            };
            await DialogService.ShowAsync<AlertDialogComponent>("Alert", parameters);
        }
    }
}
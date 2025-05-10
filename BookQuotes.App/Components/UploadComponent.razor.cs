using BookQuotes.Application.Commands;
using BookQuotes.Domain.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace BookQuotes.App.Components;

public partial class UploadComponent : ComponentBase
{
    private const int MaxFileSize = 30000000; // 30 MB
    [Inject] private IDialogService DialogService { get; set; }
    [Inject] private IAnalyseEpubFile AnalyseEpubFile { get; set; }
    [Inject] private IAnalyseQuotes AnalyseQuotes { get; set; }

    private List<Book> Books { get; set; } = new();

    private async Task UploadFiles(IReadOnlyList<IBrowserFile>? files)
    {
        if (files == null) return;

        foreach (var file in files)
        {
            if (IsEpubFile(file))
            {
                await ProcessEpubFile(file);
                continue;
            }

            if (IsAnnotationsFile(file))
            {
                await ProcessAnnotationFile(file);
                continue;
            }

            await UnknownFile(file);
        }
    }

    private async Task ProcessEpubFile(IBrowserFile file)
    {
        try
        {
            await using var stream = file.OpenReadStream(MaxFileSize);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);

            var result = await AnalyseEpubFile.Analyse(ms);

            if (result == null)
            {
                await ShowMessageBox("Error", $"File {file.Name} is not a valid epub file.");
                return;           
            }
            
            var index = Books.FindIndex(b => b.Title == result.Title);
            if (index >= 0)
            {
                Books[index] = Books[index] with { TableOfContents = result.TableOfContents };
            }
            else
            {
                Books.Add(result);
            }
        }
        catch (Exception e)
        {
            await ShowMessageBox("Error", e.Message);
        }
    }

    private async Task ProcessAnnotationFile(IBrowserFile file)
    {
        using var reader = new StreamReader(file.OpenReadStream());
        var text = await reader.ReadToEndAsync();
        var bookQuotes = AnalyseQuotes.Analyse(text);

        if (bookQuotes != null)
        {
            var index = Books.FindIndex(b => b.Title == bookQuotes.Title);
            if (index >= 0)
            {
                Books[index] = Books[index] with { Quotes = bookQuotes.Quotes };
            }
            else
            {
                Books.Add(bookQuotes);
            }
        }
        else
        {
            await ShowMessageBox("Error", $"File {file.Name} is not a valid annotation file.");
        }
    }

    private void Reset()
    {
        Books = [];
    }

    private static bool IsEpubFile(IBrowserFile file)
        => file.ContentType == "application/epub+zip" || file.Name.EndsWith(".epub");

    private static bool IsAnnotationsFile(IBrowserFile file)
        => file.Name.EndsWith(".xml") || file.Name.EndsWith(".annot");

    private async Task UnknownFile(IBrowserFile file)
        => await ShowMessageBox("Alert", $"File {file.Name} is not an epub file nor an XML (annotations) file.");

    private async Task ShowMessageBox(string title, string message)
        => await DialogService.ShowMessageBox(title, message);
}
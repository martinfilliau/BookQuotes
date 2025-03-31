using Microsoft.JSInterop;

namespace BookQuotes.App.Services;

public class FileDownloaderService(IJSRuntime jsRuntime)
{
    public async Task DownloadFileAsync(string fileName, MemoryStream memoryStream, string contentType)
    {
        var fileBytes = memoryStream.ToArray();
        var base64 = Convert.ToBase64String(fileBytes);
        await jsRuntime.InvokeVoidAsync("downloadFile", fileName, base64, contentType);
    }
}
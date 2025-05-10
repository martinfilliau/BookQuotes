using BookQuotes.Domain.Entities;
using VersOne.Epub;

namespace BookQuotes.Infrastructure.Parsers;

public class EpubParser
{
    private readonly TableOfContents _tableOfContents = new();
    
    public async Task<Book?> Parse(Stream stream)
    {
        using var ms = new MemoryStream();
        stream.Position = 0;
        await stream.CopyToAsync(ms);
        using var book = await EpubReader.OpenBookAsync(ms);

        if (book is null) return null;
        
        var title = book.Title;
        var author = book.Author;

        _tableOfContents.AddItem("ROOT", title);
        var items = await book.GetNavigationAsync();
        if (items != null)
        {
            foreach (var navigationItem in items)
            {
                AddNavigationItem(navigationItem, 0, "ROOT");
            }
        }

        return new Book
        {
            Title = title,
            Author = author,
            TableOfContents = _tableOfContents,
        };
    }
    
    private void AddNavigationItem(EpubNavigationItemRef item, int identLevel, string? parentId = null)
    {
        var filePath = item.HtmlContentFileRef?.FilePath ?? $"X{identLevel}";
        var id = GetId(filePath) ?? filePath;
        _tableOfContents.AddItem(id,
            item.Title,
            item.Link?.ContentFileUrl,
            parentId);
        foreach (var nestedNavigationItemRef in item.NestedItems)
        {
            AddNavigationItem(nestedNavigationItemRef, identLevel + 1, id);
        }
    }
    
    private static string? GetId(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl)) return null;
        
        var parts = fileUrl.Split('/');
        return parts.LastOrDefault();
    }
}
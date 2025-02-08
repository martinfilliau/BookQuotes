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
        foreach (var navigationItem in await book.GetNavigationAsync())
        {
            AddNavigationItem(navigationItem, 0, "ROOT");
        }

        return new Book
        {
            Title = title,
            Author = author,
            TableOfContents = _tableOfContents,
        };
    }
    
    private void AddNavigationItem(EpubNavigationItemRef item, int identLevel, string parentId = null)
    {
        var id = item.HtmlContentFileRef?.FilePath ?? $"X{identLevel}";
        _tableOfContents.AddItem(id,
            item.Title,
            item.Link?.ContentFileUrl,
            parentId);
        foreach (var nestedNavigationItemRef in item.NestedItems)
        {
            AddNavigationItem(nestedNavigationItemRef, identLevel + 1, id);
        }
    }
}
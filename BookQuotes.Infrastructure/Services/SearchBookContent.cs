using BookQuotes.Application.Commands;
using BookQuotes.Domain.Entities;
using BookQuotes.Domain.ValueObjects;
using BookQuotes.Infrastructure.Parsers;

namespace BookQuotes.Infrastructure.Services;

public class SearchBookContent(BookContentIndexService _indexService) : ISearchBookContent
{
    private readonly EpubParser _epubParser = new();

    public async Task IndexBook(string bookId, Stream stream, BookSearchMode searchMode)
    {
        var htmlContents = await _epubParser.ExtractHtmlContent(stream);
        await _indexService.IndexBookContent(bookId, htmlContents, searchMode);
    }

    public List<SearchResult> Search(Book book, string query, int maxResults = 10)
    {
        var results = _indexService.Search(book.Title, query, book.SearchMode, maxResults);
        
        if (book.TableOfContents == null) return results;

        foreach (var result in results)
        {
            var fileUrlParts = result.FileUrl.Split('/');
            if (fileUrlParts.Length == 0) continue;
            
            var latestFileUrl = fileUrlParts[^1]; // e.g. text/PL01.xhtml => PL01.xhtml
            
            var location = book.TableOfContents.GetItem(latestFileUrl);
            if (location != null)
            {
                result.FileTitle = location.Title;
            }
        }
        
        return results;
    }
}

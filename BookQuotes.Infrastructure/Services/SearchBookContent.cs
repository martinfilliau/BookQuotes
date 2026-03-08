using BookQuotes.Application.Commands;
using BookQuotes.Domain.Entities;
using BookQuotes.Domain.ValueObjects;
using BookQuotes.Infrastructure.Parsers;

namespace BookQuotes.Infrastructure.Services;

public class SearchBookContent : ISearchBookContent
{
    private readonly BookContentIndexService _indexService;
    private readonly EpubParser _epubParser;

    public SearchBookContent(BookContentIndexService indexService)
    {
        _indexService = indexService;
        _epubParser = new EpubParser();
    }

    public async Task IndexBook(string bookId, Stream stream, BookSearchMode searchMode)
    {
        var htmlContents = await _epubParser.ExtractHtmlContent(stream);
        await _indexService.IndexBookContent(bookId, htmlContents, searchMode);
    }

    public List<SearchResult> Search(string bookId, string query, BookSearchMode searchMode, int maxResults = 10)
    {
        return _indexService.Search(bookId, query, searchMode, maxResults);
    }
}

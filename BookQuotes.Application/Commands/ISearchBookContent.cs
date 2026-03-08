using BookQuotes.Domain.Entities;
using BookQuotes.Domain.ValueObjects;

namespace BookQuotes.Application.Commands;

public interface ISearchBookContent
{
    Task IndexBook(string bookId, Stream stream, BookSearchMode searchMode);
    List<SearchResult> Search(string bookId, string query, BookSearchMode searchMode, int maxResults = 10);
}

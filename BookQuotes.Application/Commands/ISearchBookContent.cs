using BookQuotes.Domain.Entities;
using BookQuotes.Domain.ValueObjects;

namespace BookQuotes.Application.Commands;

public interface ISearchBookContent
{
    Task IndexBook(string bookId, Stream stream, BookSearchMode searchMode);
    List<SearchResult> Search(Book book, string query, int maxResults = 10);
}

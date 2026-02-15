using BookQuotes.Domain.Entities;

namespace BookQuotes.Application.Commands;

public interface ISearchBookContent
{
    Task IndexBook(string bookId, Stream stream);
    List<SearchResult> Search(string bookId, string query, int maxResults = 10);
}

using BookQuotes.Domain.Entities;

namespace BookQuotes.Application.Commands;

public interface IExportQuotes
{
    string ExportQuotesToMarkdown(Book book, List<Quote> quotes);
}
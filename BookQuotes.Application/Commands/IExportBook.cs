using BookQuotes.Domain.Entities;

namespace BookQuotes.Application.Commands;

public interface IExportBook
{
    /// <summary>
    /// Generate a markdown file from the book
    /// </summary>
    string ExportToMarkdown(Book book, bool includeQuotes = true, bool includeTableOfContents = true);
}
using BookQuotes.Application.Commands;
using BookQuotes.Domain.Entities;
using Grynwald.MarkdownGenerator;

namespace BookQuotes.Infrastructure.Services;

public class ExportBook : IExportBook
{
    /// <inheritdoc />
    public string ExportToMarkdown(Book book, bool includeQuotes = true, bool includeTableOfContents = true)
    {
        var document = new MdDocument();
        
        document.Root.Add(new MdHeading(book.Title, 1));
        
        document.Root.Add(new MdParagraph(book.Author));

        if (includeQuotes && book.Quotes.Count > 0)
        {
            document.Root.Add(new MdHeading("Quotes", 2));
            
            foreach (var quote in book.Quotes)
            {
                var reference = quote.Reference?.Title;
                if (string.IsNullOrWhiteSpace(reference))
                {
                    document.Root.Add(new MdBlockQuote(quote.Comment));
                }
                else
                {
                    var content = $"{quote.Comment} -- {reference}";
                    document.Root.Add(new MdBlockQuote(content));
                }
            }
        }

        if (includeTableOfContents && book.TableOfContents != null)
        {
            document.Root.Add(new MdHeading("Table of contents", 2));

            var items = book.TableOfContents.GetTableOfContentsAsArray();
            foreach (var item in items)
            {
                document.Root.Add(new MdListItem(item));
            }
        }
      
        return document.ToString();
    }
}
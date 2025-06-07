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

        if (includeQuotes && book.Annotations.Count > 0)
        {
            document.Root.Add(new MdHeading("Quotes", 2));
            
            foreach (var annotation in book.Annotations)
            {
                var reference = annotation.Reference?.Title;
                if (string.IsNullOrWhiteSpace(reference))
                {
                    document.Root.Add(new MdBlockQuote(annotation.Quote));
                }
                else
                {
                    var content = $"{annotation.Quote} -- {reference}";
                    document.Root.Add(new MdBlockQuote(content));
                }

                if (!string.IsNullOrWhiteSpace(annotation.Comment))
                {
                    document.Root.Add(new MdParagraph(annotation.Comment));
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
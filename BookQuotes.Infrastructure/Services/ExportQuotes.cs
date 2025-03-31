using BookQuotes.Application.Commands;
using BookQuotes.Domain.Entities;
using Grynwald.MarkdownGenerator;

namespace BookQuotes.Infrastructure.Services;

public class ExportQuotes : IExportQuotes
{
    public string ExportQuotesToMarkdown(Book book, List<Quote> quotes)
    {
        var document = new MdDocument();
        
        document.Root.Add(new MdHeading(book.Title, 1));
        
        document.Root.Add(new MdParagraph(book.Author));

        document.Root.Add(new MdHeading("Quotes", 2));
        
        foreach (var quote in quotes)
        {
            document.Root.Add(new MdParagraph(quote.Comment));
        }
       
        return document.ToString();
    }
}
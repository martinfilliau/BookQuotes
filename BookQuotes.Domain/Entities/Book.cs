namespace BookQuotes.Domain.Entities;

public record Book
{
    public required string Title;
    public required string Author;
    public List<Quote> Quotes = [];
    public TableOfContents? TableOfContents;

    /// <summary>
    /// Update quotes with closest Table of Contents item reference
    /// </summary>
    public void UpdateQuotesWithTableOfContents()
    {
        if (TableOfContents == null) return;
        if (Quotes.Count == 0) return;
        
        foreach (var quote in Quotes)
        {
            var position = quote.Position;
            var id = position
                .Split("#", StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault()?
                .Split("/", StringSplitOptions.RemoveEmptyEntries)
                .LastOrDefault() ?? position;
            quote.Reference = TableOfContents.GetItem(id);
        }
    }
}
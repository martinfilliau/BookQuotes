namespace BookQuotes.Domain.Entities;

public record Book
{
    public required string Title;
    public required string Author;
    public List<Annotation> Annotations = [];
    public TableOfContents? TableOfContents;

    /// <summary>
    /// Update quotes with closest Table of Contents item reference
    /// </summary>
    public void UpdateQuotesWithTableOfContents()
    {
        if (TableOfContents == null) return;
        if (Annotations.Count == 0) return;
        
        foreach (var quote in Annotations)
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
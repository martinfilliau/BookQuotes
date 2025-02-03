namespace BookQuotes.Domain.Entities;

public record Book()
{
    public required string Title;
    public required string Author;
    public required List<Quote> Quotes;
};
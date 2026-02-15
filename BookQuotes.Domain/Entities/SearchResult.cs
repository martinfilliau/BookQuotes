namespace BookQuotes.Domain.Entities;

public record SearchResult
{
    public required string Content { get; init; }
    public required string HighlightedContent { get; init; }
    public required string ExpandedContent { get; init; }
    public required string FileUrl { get; init; }
    public float Score { get; init; }
}

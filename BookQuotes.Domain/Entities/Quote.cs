namespace BookQuotes.Domain.Entities;

public record Quote
{
    public required string Comment;
    
    /// <summary>
    /// "Raw" position in the book (probably a path to an HTML file)
    /// </summary>
    public required string Position;

    /// <summary>
    /// Reference to the Table of Contents item
    /// </summary>
    public TableOfContentsItem? Reference { get; set; }
};
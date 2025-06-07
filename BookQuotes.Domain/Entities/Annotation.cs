namespace BookQuotes.Domain.Entities;

public record Annotation
{
    /// <summary>
    /// Quoted text
    /// </summary>
    public required string Quote;

    /// <summary>
    /// Optional comment written by the user
    /// </summary>
    public string? Comment;
    
    /// <summary>
    /// "Raw" position in the book (probably a path to an HTML file)
    /// </summary>
    public required string Position;

    /// <summary>
    /// Reference to the Table of Contents item
    /// </summary>
    public TableOfContentsItem? Reference { get; set; }
};
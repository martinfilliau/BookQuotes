using BookQuotes.Domain.Entities;

namespace BookQuotes.Domain.Tests;

public class TableOfContentTests
{
    // Sample test data
    private TableOfContentsItem CreateTestTableOfContents()
    {
        var subsection111 = new TableOfContentsItem("", "Subsection 1.1.1", "3");
        var subsection11 = new TableOfContentsItem("", "Section 1.1", "2");
        subsection11.SubItems.Add(subsection111);
        
        var tocChapter1 = new TableOfContentsItem("", "Chapter 1", "1");
        tocChapter1.SubItems.AddRange(
            subsection11,
            new("", "Section 1.2", "4")
        );
        return tocChapter1;
    }

    [Fact]
    public void GetTableOfContentsAsArray_ShouldReturnEmptyList_WhenRootIsNull()
    {
        // Arrange
        var tableOfContents = new TableOfContents { Root = null };

        // Act
        var result = tableOfContents.GetTableOfContentsAsArray();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetTableOfContentsAsArray_ShouldReturnFormattedList_WhenRootIsNotNull()
    {
        // Arrange
        var tableOfContents = new TableOfContents
        {
            Root = CreateTestTableOfContents()
        };

        // Act
        var result = tableOfContents.GetTableOfContentsAsArray();

        // Assert
        var expected = new List<string>
        {
            "- Chapter 1 (Page: 1)",
            "  - Section 1.1 (Page: 2)",
            "    - Subsection 1.1.1 (Page: 3)",
            "  - Section 1.2 (Page: 4)"
        };

        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetTableOfContentsAsArray_ShouldHandleEmptySubItems()
    {
        // Arrange
        var tableOfContents = new TableOfContents
        {
            Root = new TableOfContentsItem("", "Chapter 1", "1")
        };

        // Act
        var result = tableOfContents.GetTableOfContentsAsArray();

        // Assert
        var expected = new List<string>
        {
            "- Chapter 1 (Page: 1)"
        };

        Assert.Equal(expected, result);
    }
}
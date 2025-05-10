using BookQuotes.Infrastructure.Parsers;

namespace BookQuotes.Infrastructure.Tests;

public class ValueCleanerTests
{
    [Theory]
    [InlineData("Simple text", "Simple text")]
    [InlineData("Text\nwith\nnewlines", "Text with newlines")]
    [InlineData("Text\rwith\rcarriage return", "Text with carriage return")]
    [InlineData("Text\r\nwith\r\nWindows newlines", "Text with Windows newlines")]
    [InlineData("   Trim spaces   ", "Trim spaces")]
    [InlineData("\n\nOnly newlines\n\n", "Only newlines")]
    [InlineData("   Mixed \n cases \r\n here   ", "Mixed cases here")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void CleanupValue_ShouldRemoveLineBreaks_AndTrimSpaces(string? input, string? expected)
    {
        // Act
        var result = ValueCleaner.CleanupValue(input);

        // Assert
        Assert.Equal(expected, result);
    }
}
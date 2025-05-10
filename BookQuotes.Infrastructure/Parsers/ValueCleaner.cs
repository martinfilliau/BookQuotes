using System.Text.RegularExpressions;

namespace BookQuotes.Infrastructure.Parsers;

public static partial class ValueCleaner
{
    private static readonly Regex LineBreaksRegex = LineBreaksOrMultipleSpacesDetectionRegex();

    public static string? CleanupValue(string? input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        return LineBreaksRegex
            .Replace(input, " ")
            .Trim();
    }

    [GeneratedRegex(@"\s+", RegexOptions.Compiled)]
    private static partial Regex LineBreaksOrMultipleSpacesDetectionRegex();
}
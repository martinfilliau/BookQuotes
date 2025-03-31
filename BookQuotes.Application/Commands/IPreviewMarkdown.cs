namespace BookQuotes.Application.Commands;

public interface IPreviewMarkdown
{
    /// <summary>
    /// Convert Markdown to HTML
    /// </summary>
    /// <param name="markdown">Markdown string</param>
    /// <returns>HTML string</returns>
    string ExportMarkdownToHtml(string markdown);
}
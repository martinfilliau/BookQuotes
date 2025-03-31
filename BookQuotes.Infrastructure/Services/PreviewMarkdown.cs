using BookQuotes.Application.Commands;
using Markdig;

namespace BookQuotes.Infrastructure.Services;

public class PreviewMarkdown : IPreviewMarkdown
{
    public string ExportMarkdownToHtml(string markdown)
    {
        return Markdown.ToHtml(markdown);
    }
}
using System.Text.RegularExpressions;
using BookQuotes.Domain.Entities;
using BookQuotes.Domain.ValueObjects;
using BookQuotes.Infrastructure.Parsers;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.En;
using Lucene.Net.Analysis.Fr;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Store;
using Lucene.Net.Util;
using QueryParser = Lucene.Net.QueryParsers.Classic.QueryParser;

namespace BookQuotes.Infrastructure.Services;

public class BookContentIndexService : IDisposable
{
    private const LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;
    private readonly RAMDirectory _indexDirectory;
    private readonly SemaphoreSlim _indexLock = new(1, 1);
    private readonly Dictionary<string, BookSearchMode> _bookAnalyzers = new();
    private IndexWriter? _writer;

    public BookContentIndexService()
    {
        _indexDirectory = new RAMDirectory();

        // Initialize writer with a default analyzer (will be replaced per document)
        var config = new IndexWriterConfig(AppLuceneVersion, new EnglishAnalyzer(AppLuceneVersion));
        _writer = new IndexWriter(_indexDirectory, config);
    }

    public async Task IndexBookContent(string bookId, List<EpubContent> htmlContents, BookSearchMode searchMode)
    {
        var analyzer = CreateAnalyzer(searchMode);
        if (analyzer == null)
        {
            // No indexing requested
            return;
        }

        await _indexLock.WaitAsync();
        try
        {
            // Store the analyzer type for this book
            _bookAnalyzers[bookId] = searchMode;

            // Clear existing documents for this book
            _writer!.DeleteDocuments(new Term("bookId", bookId));

            foreach (var epubContent in htmlContents)
            {
                var plainText = StripHtmlTags(epubContent.HtmlContent);
                if (string.IsNullOrWhiteSpace(plainText)) continue;

                var doc = new Document
                {
                    new StringField("bookId", bookId, Field.Store.YES),
                    new StringField("fileUrl", epubContent.FileUrl, Field.Store.YES),
                    new TextField("content", plainText, Field.Store.YES)
                };

                _writer.AddDocument(doc);
            }

            _writer.Commit();
        }
        finally
        {
            analyzer?.Dispose();
            _indexLock.Release();
        }
    }

    public List<SearchResult> Search(string bookId, string queryText, BookSearchMode searchMode, int maxResults = 10)
    {
        if (string.IsNullOrWhiteSpace(queryText))
            return [];

        var analyzer = CreateAnalyzer(searchMode);
        if (analyzer == null)
            return [];

        try
        {
            var results = new List<SearchResult>();

            using var reader = DirectoryReader.Open(_indexDirectory);
            var searcher = new IndexSearcher(reader);

            var parser = new QueryParser(AppLuceneVersion, "content", analyzer);
            var contentQuery = parser.Parse(queryText);

            var bookIdQuery = new TermQuery(new Term("bookId", bookId));
            var booleanQuery = new BooleanQuery
            {
                { bookIdQuery, Occur.MUST },
                { contentQuery, Occur.MUST }
            };

            var hits = searcher.Search(booleanQuery, maxResults).ScoreDocs;

            var highlighter = new Highlighter(new SimpleHTMLFormatter("<mark>", "</mark>"), new QueryScorer(contentQuery));
            var shortFragmenter = new SimpleFragmenter(200);
            var longFragmenter = new SimpleFragmenter(600);

            foreach (var hit in hits)
            {
                var doc = searcher.Doc(hit.Doc);
                var content = doc.Get("content");
                var fileUrl = doc.Get("fileUrl") ?? string.Empty;

                // Short highlighted fragment
                highlighter.TextFragmenter = shortFragmenter;
                var tokenStream1 = analyzer.GetTokenStream("content", content);
                var highlightedFragments = highlighter.GetBestFragments(tokenStream1, content, 3, "...");

                // Expanded highlighted fragment
                highlighter.TextFragmenter = longFragmenter;
                var tokenStream2 = analyzer.GetTokenStream("content", content);
                var expandedFragments = highlighter.GetBestFragments(tokenStream2, content, 3, "...");

                results.Add(new SearchResult
                {
                    Content = content,
                    HighlightedContent = string.IsNullOrWhiteSpace(highlightedFragments)
                        ? TruncateContent(content, 200)
                        : highlightedFragments,
                    ExpandedContent = string.IsNullOrWhiteSpace(expandedFragments)
                        ? TruncateContent(content, 600)
                        : expandedFragments,
                    FileUrl = fileUrl,
                    Score = hit.Score
                });
            }

            return results;
        }
        finally
        {
            analyzer?.Dispose();
        }
    }

    private static Analyzer? CreateAnalyzer(BookSearchMode mode)
    {
        return mode switch
        {
            BookSearchMode.None => null,
            BookSearchMode.French => new FrenchAnalyzer(AppLuceneVersion),
            BookSearchMode.English => new EnglishAnalyzer(AppLuceneVersion),
            _ => null
        };
    }

    private static string StripHtmlTags(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        // Remove script and style tags and their content
        html = Regex.Replace(html, @"<(script|style)[^>]*?>.*?</\1>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        // Remove HTML tags
        html = Regex.Replace(html, @"<[^>]+>", " ");

        // Remove HTML entities
        html = Regex.Replace(html, @"&[a-z]+;", " ", RegexOptions.IgnoreCase);

        // Normalize whitespace
        html = Regex.Replace(html, @"\s+", " ");

        return html.Trim();
    }

    private static string TruncateContent(string content, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(content) || content.Length <= maxLength)
            return content;

        return content[..maxLength] + "...";
    }

    public void Dispose()
    {
        _writer?.Dispose();
        _indexDirectory?.Dispose();
        _indexLock?.Dispose();
    }
}

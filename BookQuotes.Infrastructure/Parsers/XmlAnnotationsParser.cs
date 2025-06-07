using System.Xml;
using System.Xml.Linq;
using BookQuotes.Domain.Entities;

namespace BookQuotes.Infrastructure.Parsers;

public static class XmlAnnotationsParser
{
    private static readonly XNamespace Ns = "http://ns.adobe.com/digitaleditions/annotations";
    private static readonly XNamespace Dc = "http://purl.org/dc/elements/1.1/";

    public static Book? Parse(string? xmlData)
    {
        if (string.IsNullOrWhiteSpace(xmlData))
            return null;

        var doc = XDocument.Parse(xmlData);
        var publication = ExtractPublication(doc);
        var (title, author) = ExtractBookDetails(publication);
        var quotes = ExtractQuotes(doc);

        return new Book
        {
            Title = title,
            Author = author,
            Annotations = quotes
        };
    }

    private static XElement ExtractPublication(XDocument? doc)
    {
        return doc?.Root?.Element(Ns + "publication")
               ?? throw new Exception("Unable to parse publication: missing publication element");
    }

    private static (string title, string author) ExtractBookDetails(XElement publication)
    {
        var title = publication.Element(Dc + "title")?.Value
                    ?? throw new Exception("Unable to parse publication: missing title");
        var author = publication.Element(Dc + "creator")?.Value
                     ?? throw new Exception("Unable to parse publication: missing author");

        return (title, author);
    }

    private static List<Annotation> ExtractQuotes(XDocument? doc)
    {
        var quotes = new List<Annotation>();
        var annotations = doc?.Root?.Elements(Ns + "annotation");

        if (annotations is null)
            return [];

        foreach (var annotation in annotations)
            if (TryExtractQuote(annotation) is { } quote)
                quotes.Add(quote);

        return quotes;
    }

    private static Annotation? TryExtractQuote(XElement annotation)
    {
        var fragment = annotation.Element(Ns + "target")?.Element(Ns + "fragment");
        if (fragment is null) return null;

        var content = annotation.Element(Ns + "content");
        var comment = ValueCleaner.CleanupValue(content?.Element(Ns + "text")?.Value ?? null);
        
        return new Annotation
        {
            Comment  = comment,
            Quote    = ValueCleaner.CleanupValue(fragment.Element(Ns + "text")?.Value ?? "") ?? "XX",
            Position = fragment.Attribute("start")?.Value ?? ""
        };
    }
}
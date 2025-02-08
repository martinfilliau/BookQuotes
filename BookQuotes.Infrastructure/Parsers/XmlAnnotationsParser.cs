using System.Xml.Linq;
using BookQuotes.Domain.Entities;

namespace BookQuotes.Infrastructure.Parsers;

public static class XmlAnnotationsParser
{
    static XNamespace dc = "http://purl.org/dc/elements/1.1/";
    static XNamespace ns = "http://ns.adobe.com/digitaleditions/annotations";

    public static Book? Parse(string xmlData)
    {
        if (string.IsNullOrWhiteSpace(xmlData))
            return null;
        
        var bookTitle = "";
        var author = "";
        var quotes = new List<Quote>();
        
        var doc = XDocument.Parse(xmlData);

        // Extract publication details
        XElement? publication = doc?.Root?.Element(ns + "publication");
        
        if (publication == null)
            throw new Exception("Unable to parse publication");
        
        bookTitle = publication.Element(dc + "title")?.Value;
        author = publication.Element(dc + "creator")?.Value;

        // Extract annotations
        var annotations = doc?.Root?.Elements(ns + "annotation");
        
        if (annotations == null)
            return new Book
            {
                Title = bookTitle,
                Author = author,
                Quotes = []
            };
        
        foreach (var annotation in annotations)
        {
            var fragment = annotation.Element(ns + "target")?.Element(ns + "fragment");
            
            if (fragment == null) continue;
            
            var comment = fragment.Element(ns + "text")?.Value ?? "";
                
            quotes.Add(new Quote
            {
                Comment = ValueCleaner.CleanupValue(comment),
                Position = fragment.Attribute("start")?.Value ?? "",
            });
        }

        return new Book
        {
            Title = bookTitle,
            Author = author,
            Quotes = quotes,
        };
    }
}
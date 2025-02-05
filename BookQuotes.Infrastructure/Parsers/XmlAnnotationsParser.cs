using System.Xml.Linq;
using BookQuotes.Domain.Entities;

namespace BookQuotes.Infrastructure.Parsers;

public class XmlAnnotationsParser
{
    public static Book Parse(string xmlData)
    {
        var bookTitle = "";
        var author = "";
        var quotes = new List<Quote>();
        
        var doc = XDocument.Parse(xmlData);
        XNamespace dc = "http://purl.org/dc/elements/1.1/";
        XNamespace ns = "http://ns.adobe.com/digitaleditions/annotations";

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
            if (fragment != null)
            {
               quotes.Add(new Quote
                {
                    Comment = fragment.Element(ns + "text")?.Value ?? "",
                    Position = annotation.Element(dc + "identifier")?.Value ?? ""
                });
            }
        }

        return new Book
        {
            Title = bookTitle,
            Author = author,
            Quotes = quotes,
        };
    }
}
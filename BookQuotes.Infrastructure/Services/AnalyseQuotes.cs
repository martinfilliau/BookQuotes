using BookQuotes.Application.Commands;
using BookQuotes.Domain.Entities;
using BookQuotes.Infrastructure.Parsers;

namespace BookQuotes.Infrastructure.Services;

public class AnalyseQuotes : IAnalyseQuotes
{
    public List<Quote> Analyse(string xml)
    {
        var book = XmlAnnotationsParser.Parse(xml);
        return book.Quotes;
    }
}
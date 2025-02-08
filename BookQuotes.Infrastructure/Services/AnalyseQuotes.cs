using BookQuotes.Application.Commands;
using BookQuotes.Domain.Entities;
using BookQuotes.Infrastructure.Parsers;

namespace BookQuotes.Infrastructure.Services;

public class AnalyseQuotes : IAnalyseQuotes
{
    public Book? Analyse(string xml)
    {
        return XmlAnnotationsParser.Parse(xml);
    }
}
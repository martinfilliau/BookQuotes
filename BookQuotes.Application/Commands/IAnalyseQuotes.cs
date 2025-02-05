using BookQuotes.Domain.Entities;

namespace BookQuotes.Application.Commands;

public interface IAnalyseQuotes
{
    List<Quote> Analyse(string xml);
}
using BookQuotes.Domain.Entities;

namespace BookQuotes.Application.Commands;

public interface IAnalyseQuotes
{
    Book? Analyse(string xml);
}
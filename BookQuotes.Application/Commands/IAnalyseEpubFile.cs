using BookQuotes.Domain.Entities;

namespace BookQuotes.Application.Commands;

public interface IAnalyseEpubFile
{
    Task<Book?> Analyse(Stream stream);
}
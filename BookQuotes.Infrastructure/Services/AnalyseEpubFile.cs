using BookQuotes.Application.Commands;
using BookQuotes.Domain.Entities;
using BookQuotes.Infrastructure.Parsers;

namespace BookQuotes.Infrastructure.Services;

public class AnalyseEpubFile : IAnalyseEpubFile
{
    public async Task<Book> Analyse(Stream stream)
    {
        var parser = new EpubParser();
        var book = await parser.Parse(stream);
        return book;
    }
}
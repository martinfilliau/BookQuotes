using BookQuotes.Domain.Entities;
using VersOne.Epub;

namespace BookQuotes.Infrastructure.Parsers;

public class EpubParser
{
    public async Task<Book> Parse(Stream stream)
    {
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        using var book = await EpubReader.OpenBookAsync(ms);
        
        var title = book.Title;
        var author = book.Author;
        
        // XXX TODO extract summary https://os.vers.one/EpubReader/examples/example-1.html

        return new Book
        {
            Title = title,
            Author = author,
            Quotes = []
        };
    }
}
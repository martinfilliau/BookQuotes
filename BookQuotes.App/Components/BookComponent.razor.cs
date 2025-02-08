using BookQuotes.Domain.Entities;
using Microsoft.AspNetCore.Components;

namespace BookQuotes.App.Components;

public partial class BookComponent : ComponentBase
{
    [Parameter] public Book Book { get; set; }

    List<string> TocItems =>
        Book?.TableOfContents == null
            ? []
            : Book.TableOfContents.GetTableOfContentsAsArray();
}
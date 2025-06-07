using BookQuotes.Domain.Entities;
using Microsoft.AspNetCore.Components;

namespace BookQuotes.App.Components;

public partial class QuoteComponent
{
    [Parameter] public required Annotation Annotation { get; set; }
}
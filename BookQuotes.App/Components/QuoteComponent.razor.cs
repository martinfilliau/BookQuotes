using BookQuotes.Domain.Entities;
using Microsoft.AspNetCore.Components;

namespace BookQuotes.App.Components;

public partial class QuoteComponent
{
    [Parameter] public required Quote Quote { get; set; }
}
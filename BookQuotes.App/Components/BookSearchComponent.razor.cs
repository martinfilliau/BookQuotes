using BookQuotes.Application.Commands;
using BookQuotes.Domain.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace BookQuotes.App.Components;

public partial class BookSearchComponent : ComponentBase
{
    [Parameter] public required Book Book { get; set; }
    [Parameter] public required Stream? EpubStream { get; set; }

    [Inject] private ISearchBookContent SearchBookContent { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    private string SearchQuery { get; set; } = string.Empty;
    private List<SearchResult> SearchResults { get; set; } = [];
    private HashSet<int> ExpandedResults { get; set; } = [];
    private bool IsSearching { get; set; }
    private bool HasSearched { get; set; }
    private bool IsIndexed { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (EpubStream != null && !IsIndexed)
        {
            await IndexBook();
        }
    }

    private async Task IndexBook()
    {
        if (EpubStream == null) return;

        try
        {
            await SearchBookContent.IndexBook(Book.Title, EpubStream);
            IsIndexed = true;
        }
        catch (Exception e)
        {
            await DialogService.ShowMessageBoxAsync("Error", $"Failed to index book: {e.Message}");
        }
    }

    private async Task PerformSearch()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            SearchResults = [];
            ExpandedResults = [];
            HasSearched = false;
            return;
        }

        IsSearching = true;
        HasSearched = false;

        try
        {
            await Task.Delay(100); // Small delay for UI responsiveness
            SearchResults = SearchBookContent.Search(Book.Title, SearchQuery);
            ExpandedResults = [];
            HasSearched = true;
        }
        catch (Exception e)
        {
            await DialogService.ShowMessageBoxAsync("Error", $"Search failed: {e.Message}");
            SearchResults = [];
            ExpandedResults = [];
        }
        finally
        {
            IsSearching = false;
        }
    }

    private void ToggleExpand(int index)
    {
        if (ExpandedResults.Contains(index))
        {
            ExpandedResults.Remove(index);
        }
        else
        {
            ExpandedResults.Add(index);
        }
    }

    private async Task HandleKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await PerformSearch();
        }
    }
}

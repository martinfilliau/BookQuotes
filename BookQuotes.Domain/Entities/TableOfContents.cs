namespace BookQuotes.Domain.Entities;

/// <summary>
/// See ADR 002
/// </summary>
public class TableOfContents
{
    private readonly Dictionary<string, TableOfContentsItem> _items = new();
    public TableOfContentsItem? Root { get; set; }

    public void AddItem(string id, string title, string? pageReference = null, string? parentId = null)
    {
        var newItem = new TableOfContentsItem(id, title, pageReference);
        _items.TryAdd(id, newItem);

        if (parentId == null)
        {
            Root = newItem; // Set root if it's a top-level item
        }
        else if (_items.TryGetValue(parentId, out var parent))
        {
            parent.SubItems.Add(newItem);
        }
    }

    public TableOfContentsItem? GetItem(string id) => _items.GetValueOrDefault(id);
   
    public List<string> GetTableOfContentsAsArray()
    {
        List<string> result = new();
        if (Root != null)
        {
            PopulateArray(Root, 0, result);
        }
        return result;
    }

    private static void PopulateArray(TableOfContentsItem item, int depth, List<string> result, char separator = ' ')
    {
        result.Add($"{new string(separator, depth * 2)}- {item.Title}");
        foreach (var subItem in item.SubItems)
        {
            PopulateArray(subItem, depth + 1, result, separator);
        }
    }
}

public class TableOfContentsItem(string id, string title, string? pageReference = null)
{
    public string Id { get; } = id;
    public string Title { get; } = title;
    public string? PageReference { get; } = pageReference;
    public List<TableOfContentsItem> SubItems { get; } = new();
}

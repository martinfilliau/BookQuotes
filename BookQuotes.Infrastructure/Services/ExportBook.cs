using BookQuotes.Application.Commands;
using BookQuotes.Domain.Entities;
using Grynwald.MarkdownGenerator;

namespace BookQuotes.Infrastructure.Services;

public class ExportBook : IExportBook
{
    private MdDocument _document = null!;

    /// <inheritdoc />
    public string ExportToMarkdown(Book book, bool includeQuotes = true, bool includeTableOfContents = true)
    {
        _document = new();
        
        _document.Root.Add(new MdHeading(book.Title, 1));
        
        _document.Root.Add(new MdParagraph(book.Author));

        if (includeQuotes && book.Annotations.Count > 0)
        {
            _document.Root.Add(new MdHeading("Quotes", 2));
            
            foreach (var annotation in book.Annotations)
            {
                AddAnnotationWithReference(annotation);
                AddAnnotationComment(annotation);
            }
        }

        if (includeTableOfContents && book.TableOfContents != null)
        {
            _document.Root.Add(new MdHeading("Table of contents", 2));

            MdBulletList parts = new();
            foreach (var item in book.TableOfContents.Root?.SubItems ?? [])
            {
                parts.Add(new MdListItem(GetSubitemsList(item)));
            }

            _document.Root.Add(parts);
        }
      
        return _document.ToString();
    }
    
    private void AddAnnotationWithReference(Annotation annotation)
    {
        var reference = annotation.Reference?.Title;
        var content = string.IsNullOrWhiteSpace(reference) 
            ? annotation.Quote 
            : $"{annotation.Quote} -- {reference}";
    
        _document.Root.Add(new MdBlockQuote(content));
    }

    /// <summary>
    /// Comment is optional and might not be present at all
    /// </summary>
    private void AddAnnotationComment(Annotation annotation)
    {
        if (!string.IsNullOrWhiteSpace(annotation.Comment))
        {
            _document.Root.Add(new MdParagraph(annotation.Comment));
        }
    }
    
    private MdListItem GetSubitemsList(TableOfContentsItem item)
    {
        var subList = item.SubItems
            .Select(subItem => new MdListItem(GetSubitemsList(subItem))).ToList();
        
        return subList.Count == 0
            ? new MdListItem(new MdParagraph(item.Title))
            : new MdListItem(new MdParagraph(item.Title), new MdBulletList(subList));
    }
}
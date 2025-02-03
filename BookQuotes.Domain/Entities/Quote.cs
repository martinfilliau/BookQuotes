namespace BookQuotes.Domain.Entities;

public record Quote()
{
    public required string Comment;
    public required string Position;
};
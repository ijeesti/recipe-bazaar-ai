namespace RecipeBazaarAi.Infrastructure.Azure.Indexes;

public record SearchResultResponse
{
    public int TotalCount { get; init; }
    public ICollection<RecipeIndex> Items { get; init; } = [];
}
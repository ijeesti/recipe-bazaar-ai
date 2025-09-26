namespace RecipeBazaarAi.Infrastructure.Azure.Indexes;

public class RecipeIndexResult
{
    public int TotalCount { get; set; }
    public ICollection<RecipeIndex> Recipes { get; set; } = [];
}
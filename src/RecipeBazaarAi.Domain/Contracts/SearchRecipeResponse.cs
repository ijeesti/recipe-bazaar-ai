using RecipeBazaarAi.Domain.Entities;

namespace RecipeBazaarAi.Domain.Contracts;

public record SearchRecipeResponse
{
    public int TotalCount { get; init; }
    public ICollection<RecipeEntity> Items { get; init; } = [];
}
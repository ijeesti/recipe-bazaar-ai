using RecipeBazaarAi.Domain.Entities;

namespace RecipeBazaarAi.Domain.Entities;

public class CategoryEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }

    public ICollection<RecipeEntity> Recipes { get; set; } = [];
}
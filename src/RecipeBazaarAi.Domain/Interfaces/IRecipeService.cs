using RecipeBazaarAi.Domain.Contracts;

namespace RecipeBazaarAi.Domain.Interfaces;

public interface IRecipeService
{
    Task<IEnumerable<RecipeDto>> GetAllRecipesAsync();
    Task<RecipeDto?> GetRecipeByIdAsync(int id);
    Task<int> CreateRecipeAsync(CreateRecipeDto createRecipeDto);
    Task<RecipeDto?> UpdateRecipeAsync(RecipeDto recipeDto);
    Task<IEnumerable<CommentDto>> GetCommentsByRecipeIdAsync(int recipeId);
}

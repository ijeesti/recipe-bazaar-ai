using RecipeBazaarAi.Domain.Contracts;

namespace RecipeBazaarAi.Domain.Interfaces;

public interface IRecipeAiService
{
    Task<bool> CreateRecipeAsync(RecipeCreateIndexDto createRecipeDto);
    Task<IEnumerable<RecipeIndexDto>> GetAllRecipesAsync(int top = 10);
    Task<IEnumerable<RecipeIndexDto>> SearchRecipesAsync(string query, int top = 10, CancellationToken ct = default);
    Task<ICollection<string>> SuggestAsync(string term, CancellationToken ct = default);
    Task<bool> AddCommentAsync(string recipeId, CommentDto comment, CancellationToken ct = default);
    Task<RecipeIndexDto?> GetByIdAsync(string recipeId, CancellationToken ct = default);
}
using RecipeBazaarAi.Infrastructure.Azure.Indexes;

namespace RecipeBazaarAi.WebApp.Interfaces;

public interface IRecipeWebService
{
    Task<RecipeIndexResult?> GetAllRecipesAsync(int take, int skip);
    Task<bool> AddCommentAsync(string recipeId, CommentIndex comment);
}
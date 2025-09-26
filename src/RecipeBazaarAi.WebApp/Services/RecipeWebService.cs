using RecipeBazaarAi.Infrastructure.Azure.Indexes;
using RecipeBazaarAi.WebApp.Interfaces;
using System.Net.Http.Json;

namespace RecipeBazaarAi.WebApp.Services;

public class RecipeWebService(HttpClient httpClient) : IRecipeWebService
{
    public async Task<bool> AddCommentAsync(string recipeId, CommentIndex comment)
    {
        var response = await httpClient.PostAsJsonAsync($"api/recipes/{Uri.EscapeDataString(recipeId)}/comments", comment);
        return response.IsSuccessStatusCode;
    }

    public async Task<RecipeIndexResult?> GetAllRecipesAsync(int take, int skip) =>
        await httpClient.GetFromJsonAsync<RecipeIndexResult>($"api/recipes/all?take={take}&skip={skip}");

    // Implement other interface methods similarly
}


using RecipeBazaarAi.Domain.Contracts;
using RecipeBazaarAi.Infrastructure.Azure.Indexes;

namespace RecipeBazaarAi.Infrastructure.Azure.Interfaces;

public interface IRecipeIndexService
{
    Task<bool> CreateOrUpdateIndexAsync();
    Task<bool> UploadRecipesAsync(string jsonFilePath);

    Task<ICollection<RecipeIndex>> GetAllRecipesAsync(int skip, int top);

    // Basic search
    Task<ICollection<RecipeIndex>> SearchRecipesAsync(SearchQueryRequest searchRequest);

    // Search with filters
    Task<ICollection<RecipeIndex>> SearchRecipesWithFiltersAsync(string query, string? category = null, string? user = null);

    // Autocomplete / suggestions
    Task<ICollection<SuggestRecipeResult>> SuggestRecipesAsync(string term);

    // Popular / boosted search
    Task<ICollection<RecipeIndex>> SearchPopularRecipesAsync(SearchQueryRequest searchRequest);

    //Weight Search
    Task<ICollection<RecipeIndex>> SearchRecipesWeightedAsync(SearchQueryRequest searchRequest);

    // Semantic / AI-based search (future-ready)
    Task<ICollection<RecipeIndex>> SemanticSearchAsync(SearchQueryRequest searchRequest);
}


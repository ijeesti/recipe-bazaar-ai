
using RecipeBazaarAi.Domain.Contracts;
using RecipeBazaarAi.Infrastructure.Azure.Indexes;

namespace RecipeBazaarAi.Infrastructure.Azure.Interfaces;

public interface IRecipeIndexService
{
    Task<bool> CreateOrUpdateIndexAsync();
    Task<bool> UploadRecipesAsync(string jsonFilePath);

    // Basic search
    Task<ICollection<RecipeIndex>> SearchRecipesAsync(SearchRequest searchRequest);

    // Search with filters
    Task<ICollection<RecipeIndex>> SearchRecipesWithFiltersAsync(string query, string? category = null, string? user = null);

    // Autocomplete / suggestions
    Task<ICollection<SuggestRecipeResult>> SuggestRecipesAsync(string term);

    // Popular / boosted search
    Task<ICollection<RecipeIndex>> SearchPopularRecipesAsync(SearchRequest searchRequest);

    //Weight Search
    Task<ICollection<RecipeIndex>> SearchRecipesWeightedAsync(SearchRequest searchRequest);

    // Semantic / AI-based search (future-ready)
    Task<ICollection<RecipeIndex>> SemanticSearchAsync(SearchRequest searchRequest);
}


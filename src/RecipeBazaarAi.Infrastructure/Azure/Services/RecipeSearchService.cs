using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Options;
using RecipeBazaarAi.Domain.Contracts;
using RecipeBazaarAi.Infrastructure.Azure.Indexes;
using RecipeBazaarAi.Infrastructure.Azure.Interfaces;
using System.Text.Json;
using FieldBuilder = Azure.Search.Documents.Indexes.FieldBuilder;

namespace RecipeBazaarAi.Infrastructure.Azure.Services;

public class RecipeSearchService(
    IOptions<AzureOptions> options,
    SearchClient searchClient,
    SearchIndexClient indexClient) : IRecipeIndexService
{
    public async Task<bool> CreateOrUpdateIndexAsync()
    {
        var indexName = options.Value.IndexName;
        if (string.IsNullOrWhiteSpace(indexName)) //example of validation :)
        {
            return false;
        }

        var fieldBuilder = new FieldBuilder();
        var fields = fieldBuilder.Build(typeof(RecipeIndex));

        var suggester = new SearchSuggester(
            name: "sg",
            sourceFields: ["Title", "Description", "Ingredients"]
        );

        var scoringProfile = new ScoringProfile("weightedProfile")
        {
            TextWeights = new TextWeights(new Dictionary<string, double>
            {
                { "Title", 3.0 },
                { "Description", 2.0 },
                { "Ingredients", 1.0 }
            })
        };

        var definition = new SearchIndex(indexName)
        {
            Fields = fields,
            Suggesters = { suggester },
            ScoringProfiles = { scoringProfile },
            DefaultScoringProfile = "weightedProfile"
        };

        await indexClient.CreateOrUpdateIndexAsync(definition);

        return true;
    }


    public async Task<bool> UploadRecipesAsync(string json)
    {
        var recipes = JsonSerializer.Deserialize<List<RecipeIndex>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (recipes is null || recipes.Count == 0)
        {
            throw new InvalidOperationException("No recipes found to upload.");
        }

        // Use MergeOrUpload for idempotency
        await searchClient.MergeOrUploadDocumentsAsync(recipes);
        return true;
    }


    public async Task<ICollection<RecipeIndex>> SearchRecipesAsync(SearchRequest searchRequest) =>
        await GetResultAsync(searchRequest.Query,
            new SearchOptions
            {
                Size = searchRequest.Top
            });

    public async Task<ICollection<RecipeIndex>> SearchRecipesWithFiltersAsync(string query, string? category = null, string? user = null)
    {
        var options = new SearchOptions { Filter = BuildFilter(category, user) };
        return await GetResultAsync(query, options);
    }

    public async Task<ICollection<SuggestRecipeResult>> SuggestRecipesAsync(string term)
    {
        var response = await searchClient.SuggestAsync<RecipeIndex>(term, "sg", new SuggestOptions { Size = 5 });
        var suggestions = new List<SuggestRecipeResult>();
        foreach (var s in response.Value.Results)
        {
            suggestions.Add(new SuggestRecipeResult
            {
                Content = s.Text,
                RecipeId = s.Document.Id,
            });
        }

        return suggestions;
    }

    public async Task<ICollection<RecipeIndex>> SearchPopularRecipesAsync(SearchRequest searchRequest)
    {
        var options = new SearchOptions { Size = searchRequest.Top, OrderBy = { "CreatedOn desc" } };
        return await GetResultAsync(searchRequest.Query, options);
    }


    public async Task<ICollection<RecipeIndex>> SearchRecipesWeightedAsync(SearchRequest searchRequest)
    {
        if (string.IsNullOrWhiteSpace(searchRequest.Query) || searchRequest.Query.Length < 3)
        {
            return Array.Empty<RecipeIndex>();
        }

        var query = $"{searchRequest.Query}*"; // prefix search

        // Configure search options
        var options = new SearchOptions
        {
            Size = searchRequest.Top,
            ScoringProfile = "weightedProfile", // scoring profile defined in your index,
            SearchMode = SearchMode.Any
        };

        // Search in all relevant fields
        options.SearchFields.Add("Title");
        options.SearchFields.Add("Description");
        options.SearchFields.Add("Ingredients");
        options.QueryType = SearchQueryType.Simple;

        return await GetResultAsync(query, options);
    }

    public async Task<ICollection<RecipeIndex>> SemanticSearchAsync(SearchRequest searchRequest)
    {
        // Placeholder for future AI / semantic ranking logic
        return await SearchRecipesAsync(searchRequest);
    }

    private async Task<ICollection<RecipeIndex>> GetResultAsync(string query, SearchOptions options)
    {
        var results = new List<RecipeIndex>(); // initialize list

        var response = searchClient.Search<RecipeIndex>(query, options);

        await foreach (var r in response.Value.GetResultsAsync())
        {
            results.Add(r.Document);
        }

        return results;

        // alternate choice:   await response.Value.GetResultsAsync()
        //.Select(r => r.Document)
        //.ToListAsync();
    }

    private static string BuildFilter(string? category, string? user)
    {
        var filters = new List<string>();
        if (!string.IsNullOrEmpty(category))
        {
            filters.Add($"Category eq '{category}'");
        }

        if (!string.IsNullOrEmpty(user))
        {
            filters.Add($"UserName eq '{user}'");
        }

        return string.Join(" and ", filters);
    }
}


//public async Task<ICollection<RecipeHighlightIndex>> SearchRecipesWithHighlightAsync(string query, int top = 10)
//{
//    var options = new SearchOptions
//    {
//        Size = top,
//        HighlightPreTag = "<b>",
//        HighlightPostTag = "</b>",
//        HighlightFields = { "Title", "Description", "Ingredients", "Instructions" }
//    };

//    var results = new List<RecipeHighlightIndex>();
//    var response = _searchClient.Search<RecipeIndex>(query, options);

//    await foreach (var r in response.Value.GetResultsAsync())
//    {
//        var doc = r.Document;
//        var highlighted = new RecipeHighlightIndex
//        {
//            Id = doc.Id,
//            Title = doc.Title,
//            Description = doc.Description,
//            Ingredients = doc.Ingredients,
//            Instructions = doc.Instructions,
//            UserName = doc.UserName,
//            CategoryName = doc.CategoryName,
//            CreatedOn = doc.CreatedOn,
//            Comments = doc.Comments,
//            Highlights = r.Highlights
//        };
//        results.Add(highlighted);
//    }

//    return results;
//}

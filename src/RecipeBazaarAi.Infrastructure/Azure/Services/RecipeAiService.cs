using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using RecipeBazaarAi.Domain.Contracts;
using RecipeBazaarAi.Domain.Interfaces;
using RecipeBazaarAi.Infrastructure.Azure.Indexes;

namespace RecipeBazaarAi.Infrastructure.Azure.Services;

public class RecipeAiService(SearchClient searchClient) : IRecipeAiService
{

    public async Task<bool> CreateRecipeAsync(RecipeCreateIndexDto createRecipeDto)
    {
        var recipe = new RecipeIndex
        {
            Title = createRecipeDto.Title,
            Description = createRecipeDto.Description,
            Ingredients = createRecipeDto.Ingredients,
            Instructions = createRecipeDto.Instructions,
            UserName = createRecipeDto.UserName,
            ImageUrl = createRecipeDto.ImageUrl ?? string.Empty,
            CategoryName = createRecipeDto.CategoryName,
        };

        var batch = IndexDocumentsBatch.MergeOrUpload([recipe]);
        var result = await searchClient.IndexDocumentsAsync(batch);
        return result != null;
    }

    public async Task<IEnumerable<RecipeIndexDto>> GetAllRecipesAsync(int top = 10)
    {
        var options = new SearchOptions
        {
            Size = top
        };
        var response = await searchClient.SearchAsync<RecipeIndex>("*", options);
        return await GetSearchResult(response);

    }


    public async Task<IEnumerable<RecipeIndexDto>> SearchRecipesAsync(string query, int top = 10, CancellationToken ct = default)
    {
        var options = new SearchOptions { Size = top };
        var response = await searchClient.SearchAsync<RecipeIndex>(query, options, ct);
        return await GetSearchResult(response);
    }

    public Task<RecipeIndexDto?> GetByIdAsync(string recipeId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> AddCommentAsync(string recipeId, CommentDto comment, CancellationToken ct = default)
    {
        // var recipe = await GetByIdAsync(recipeId, ct);
        var recipe = await searchClient.GetDocumentAsync<RecipeIndex>(recipeId);
        if (recipe == null)
        {
            return false;
        }

        var list = recipe.Value.Comments.ToList(); //Alter option to create separate DTO or to convert array to list<>..
        list.Add(new CommentIndex { Content = comment.Content, UserName = comment.UserName });   // convert array → List
        recipe.Value.Comments = [.. list];

        var batch = IndexDocumentsBatch.MergeOrUpload([recipe]);
        await searchClient.IndexDocumentsAsync(batch, cancellationToken: ct);
        return true;
    }

    public Task<ICollection<string>> SuggestAsync(string term, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    private static async Task<IEnumerable<RecipeIndexDto>> GetSearchResult(global::Azure.Response<SearchResults<RecipeIndex>> response)
    {
        var results = new List<RecipeIndexDto>();
        await foreach (var result in response.Value.GetResultsAsync())
        {
            if (result.Document != null)
            {
                var doc = result.Document;
                var dto = new RecipeIndexDto
                {
                    Title = doc.Title,
                    Description = doc.Description,
                    Ingredients = doc.Ingredients,
                    Instructions = doc.Instructions,
                    UserName = doc.UserName,
                    CategoryName = doc.CategoryName,
                    ImageUrl =string.IsNullOrEmpty(doc.ImageUrl) ? @"https://placehold.co/600x400" : doc.ImageUrl,
                    Comments = [.. doc.Comments.Select(c => new CommentDto { Content = c.Content, UserName = c.UserName })]
                };
                results.Add(dto);
            }
        }

        return results;
    }

}


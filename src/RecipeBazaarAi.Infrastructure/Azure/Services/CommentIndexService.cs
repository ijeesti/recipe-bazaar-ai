using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using RecipeBazaarAi.Infrastructure.Azure.Indexes;
using RecipeBazaarAi.Infrastructure.Azure.Interfaces;

namespace RecipeBazaarAi.Infrastructure.Azure.Services;

public class CommentIndexService (SearchClient searchClient) : ICommentIndexService
{
    public async Task<bool> AddCommentAsync(string recipeId, CommentIndex newComment)
    {

        // TODO: Save in DB: ICommentService.AddAsync();

        var searchOptions = new SearchOptions
        {
            Filter = $"Id eq '{recipeId}'"
        };

        var searchResult = await searchClient.SearchAsync<RecipeIndex>("*", searchOptions);
        var recipe = searchResult.Value.GetResults().FirstOrDefault()?.Document;

        if (recipe is null)
        {
            return false; // Recipe not found
        }

        // 2. Add the new comment
        var comments = recipe.Comments?.ToList() ?? [];
        comments.Add(newComment);
        recipe.Comments = [.. comments];

        // 3. Push updated document back into index
        var batch = IndexDocumentsBatch.MergeOrUpload([recipe]);
        await searchClient.IndexDocumentsAsync(batch);

        return true;
    }
}
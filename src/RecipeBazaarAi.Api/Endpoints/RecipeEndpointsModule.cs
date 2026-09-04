using Carter;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeBazaarAi.Domain.Contracts;
using RecipeBazaarAi.Infrastructure.Azure.Indexes;
using RecipeBazaarAi.Infrastructure.Azure.Interfaces;

namespace RecipeBazaarAi.Api.Endpoints;

public class RecipeEndpointsModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/recipes")
            .WithTags("Recipe Search");

        // GET: /api/recipes/all
        group.MapGet("/all", GetAllRecipes)
             .WithName("GetAll")
             .WithSummary("Fetch all recipe fields")
             .WithDescription("Get all recipes ordered by date created")
             .Produces<PaginationRequest>(StatusCodes.Status200OK)
             .ProducesValidationProblem(StatusCodes.Status400BadRequest)
             .ProducesProblem(StatusCodes.Status401Unauthorized)
             .ProducesProblem(StatusCodes.Status403Forbidden)
             .ProducesProblem(StatusCodes.Status404NotFound)
             .ProducesProblem(StatusCodes.Status500InternalServerError);

        // GET: /api/recipes/fulltext-search
        group.MapGet("/fulltext-search", SearchFullText)
             .WithName("SearchFullText")
             .WithSummary("Full text search across all recipe fields")
             .WithDescription("Runs a general search across titles, descriptions, ingredients and instructions");

        // GET: /api/recipes/suggest-recipe
        group.MapGet("/suggest-recipe", SuggestRecipe)
             .WithName("SuggestRecipe")
             .WithSummary("Suggest recipes while typing")
             .WithDescription("Returns recipe suggestions for auto-complete as users type search terms");

        // GET: /api/recipes/weight-search
        group.MapGet("/weight-search", SearchByFieldOrder)
             .WithName("SearchByFieldOrder")
             .WithSummary("Search recipes with field boosting")
             .WithDescription("Search that ranks results by giving higher weight to fields like Title and Description");

        // POST: /api/recipes/{id}/comments
        group.MapPost("/{id}/comments", AddComment)
             .WithName("AddComment")
             .WithSummary("Add a comment to a recipe")
             .WithDescription("Pushes a new comment into Azure Search index for a given recipe");
    }

    private async Task<Results<Ok<RecipeIndexResult>, ProblemHttpResult>> GetAllRecipes(
     [AsParameters] PaginationRequest request,
     IRecipeIndexService searchService)
    {
        int validSkip = Math.Max(0, request.Skip);
        int validTake = Math.Clamp(request.Take, 1, 100);

        var results = await searchService.GetAllRecipesAsync(validSkip, validTake);

        return TypedResults.Ok(new RecipeIndexResult
        {
            TotalCount = results.Count,
            Recipes = results ?? Array.Empty<RecipeIndex>()
        });
    }

    private async Task<Results<
        Ok<RecipeIndexResult>,
        NotFound<ProblemDetails>,
        ProblemHttpResult>> SearchFullText(
       [AsParameters] SearchQueryRequest searchQuery,
       IRecipeIndexService searchService)
    {
        var results = await searchService.SearchRecipesAsync(new SearchQueryRequest
        {
            Query = searchQuery.Query,
            Top = searchQuery.Top
        });

        // Mock 404 Not Found: No matching records found in Azure Search
        if (results is null || results.Count == 0)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "No Recipes Found",
                Detail = $"No recipes were found matching the query term '{searchQuery.Query}'.",
                Instance = "/api/recipes/fulltext-search"
            });
        }

        return TypedResults.Ok(new RecipeIndexResult
        {
            TotalCount = results.Count,
            Recipes = results ?? Array.Empty<RecipeIndex>()
        });
    }

    private async Task<IResult> SuggestRecipe(
        string term,
        IRecipeIndexService searchService)
    {
        if (string.IsNullOrWhiteSpace(term) || term.Length < 3)
        {
            return TypedResults.BadRequest("Query must be at least 3 characters.");
        }

        var results = await searchService.SuggestRecipesAsync(term);

        return TypedResults.Ok(new
        {
            TotalCount = results.Count,
            Items = results
        });
    }

    private async Task<IResult> SearchByFieldOrder(
       [AsParameters] SearchQueryRequest searchQuery,
       IRecipeIndexService searchService)
    {
        var results = await searchService.SearchRecipesWeightedAsync(searchQuery);

        return TypedResults.Ok(new
        {
            TotalCount = results.Count,
            Items = results
        });
    }

    private async Task<IResult> AddComment(
        string id,
        CommentIndex newComment,
        ICommentIndexService commentIndexService)
    {
        if (newComment == null || string.IsNullOrWhiteSpace(newComment.UserName))
        {
            return TypedResults.BadRequest("Invalid comment.");
        }

        var updated = await commentIndexService.AddCommentAsync(id, newComment);

        return updated
            ? TypedResults.Ok("Comment added successfully.")
            : TypedResults.NotFound("Recipe not found.");
    }
}
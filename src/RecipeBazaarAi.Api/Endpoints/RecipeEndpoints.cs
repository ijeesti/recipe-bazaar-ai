using Microsoft.AspNetCore.Mvc;
using RecipeBazaarAi.Domain.Contracts;
using RecipeBazaarAi.Infrastructure.Azure.Indexes;
using RecipeBazaarAi.Infrastructure.Azure.Interfaces;

namespace RecipeBazaarAi.Api.Endpoints;

public static class RecipeEndpoints
{
    public static void MapRecipeEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/recipes")
                       .WithTags("Recipe Search");


        // Full text search
        group.MapGet("/all", async (
            [FromQuery] int skip,
            [FromQuery] int take,
            IRecipeIndexService searchService) =>
        {
            take = take > 0 ? take : 10;
            skip = take > 0 ? skip : 0;
            var results = await searchService.GetAllRecipesAsync(skip, take);

            return Results.Ok(new RecipeIndexResult
            {
                TotalCount = results.Count,
                Recipes = results
            });
        })
        .WithName("GetAll")
        .WithSummary("Fetch all recipe fields")
        .WithDescription("Get all recipes order by date created")
        .Produces(StatusCodes.Status400BadRequest)
        .Produces<List<RecipeIndex>>(StatusCodes.Status200OK);

        // Full text search
        group.MapGet("/fulltext-search", async (
            [FromQuery] string query,
            [FromQuery] int top,
            IRecipeIndexService searchService) =>
        {
            if (!string.IsNullOrWhiteSpace(query) && query.Length >= 3)
            {
                top = top > 0 ? top : 10;

                var results = await searchService.SearchRecipesAsync(new SearchRequest { Query = query, Top = top });

                return Results.Ok(new
                {
                    TotalCount = results.Count,
                    Items = results
                });
            }

            return Results.BadRequest("Query must be at least 3 characters.");
        })
        .WithName("SearchFullText")
        .WithSummary("Full text search across all recipe fields")
        .WithDescription("Runs a general search across titles, descriptions, ingredients and instructions")
        .Produces(StatusCodes.Status400BadRequest)
        .Produces<List<RecipeIndex>>(StatusCodes.Status200OK);

        // Suggestions (type-ahead search)
        group.MapGet("/suggest-recipe", async (
            [FromQuery] string term,
            IRecipeIndexService searchService) =>
        {
            if (!string.IsNullOrWhiteSpace(term))
            {
                var results = await searchService.SuggestRecipesAsync(term);

                return Results.Ok(new
                {
                    TotalCount = results.Count,
                    Items = results
                });
            }

            return Results.BadRequest("Query must be at least 3 characters.");
        })
        .WithName("SuggestRecipe")
        .WithSummary("Suggest recipes while typing")
        .WithDescription("Returns recipe suggestions for auto-complete as users type search terms")
        .Produces(StatusCodes.Status400BadRequest)
        .Produces<List<RecipeIndex>>(StatusCodes.Status200OK);

        // Weighted search
        group.MapGet("/weight-search", async (
            [FromQuery] string query,
            [FromQuery] int top,
            IRecipeIndexService searchService) =>
        {
            if (!string.IsNullOrWhiteSpace(query) && query.Length >= 3)
            {
                top = top > 0 ? top : 10;
                var results = await searchService.SearchRecipesWeightedAsync(new SearchRequest { Query = query, Top = top });

                return Results.Ok(new
                {
                    TotalCount = results.Count,
                    Items = results
                });
            }

            return Results.BadRequest("Query must be at least 3 characters.");
        })
        .WithName("SearchByFieldOrder")
        .WithSummary("Search recipes with field boosting")
        .WithDescription("Search that ranks results by giving higher weight to fields like Title and Description")
        .Produces(StatusCodes.Status400BadRequest)
        .Produces<List<RecipeIndex>>(StatusCodes.Status200OK);

        // Add comment to a recipe
        group.MapPost("/{id}/comments", async (
            [FromRoute] string id,
            [FromBody] CommentIndex newComment,
            [FromServices] ICommentIndexService commentIndexService) =>
        {
            if (newComment == null || string.IsNullOrWhiteSpace(newComment.UserName))
            {
                return Results.BadRequest("Invalid comment.");
            }

            var updated = await commentIndexService.AddCommentAsync(id, newComment);

            return updated
                ? Results.Ok("Comment added successfully.")
                : Results.NotFound("Recipe not found.");
        })
        .WithName("AddComment")
        .WithSummary("Add a comment to a recipe")
        .WithDescription("Pushes a new comment into Azure Search index for a given recipe")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);
    }
}

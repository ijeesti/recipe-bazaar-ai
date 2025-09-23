using Microsoft.AspNetCore.Mvc;
using RecipeBazaarAi.Infrastructure.Azure.Interfaces;

namespace RecipeBaazar.Api.Endpoints;

public static class IndexerEndpoints
{
    public static void MapIndexEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/index")
                       .WithTags("Json Indexer");

        group.MapPost("/create", async ([FromServices] IRecipeIndexService searchService) =>
        {
            var success = await searchService.CreateOrUpdateIndexAsync();
            return success
                ? Results.Ok("Index created or updated successfully.")
                : Results.Problem("Failed to create index.");
        })
            .WithName("CreateOrUpdateIndex")
            .WithSummary("Creates or updates the Azure Search index for recipes.")
            .WithDescription("Call this only once or when the schema changes.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/upload-data", async (IFormFile file, IRecipeIndexService searchService) =>
        {
            if (file == null || file.Length == 0)
            {
                return Results.BadRequest("No file uploaded.");
            }

            using var reader = new StreamReader(file.OpenReadStream());
            var json = await reader.ReadToEndAsync();

            var uploaded = await searchService.UploadRecipesAsync(json);
            return uploaded
                ? Results.Ok("Recipes uploaded successfully.")
                : Results.BadRequest("Data already uploaded.");
        })
            .Accepts<IFormFile>("multipart/form-data")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName("UploadJsonData")
            .WithSummary("Uploads JSON to Azure Search index.")
            .WithDescription("Upload a JSON file containing recipes.")
            .DisableAntiforgery();//demo
    }
}

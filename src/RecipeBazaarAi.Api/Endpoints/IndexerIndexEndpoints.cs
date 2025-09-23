using Microsoft.AspNetCore.Mvc;
using RecipeBazaarAi.Infrastructure.Azure.Interfaces;


namespace RecipeBaazar.Api.Endpoints;

public static class IndexerIndexEndpoints
{
    public static void MapIndexerIndexEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/indexer")
                       .WithTags("Create Index with Indexer");

        group.MapPost("/create", async ([FromServices] IRecipeIndexService searchService) =>
        {
            var success = await searchService.CreateOrUpdateIndexAsync();
            return success
                ? Results.Ok("Index created or updated successfully.")
                : Results.Problem("Failed to create index.");
        })
            .WithName("CreateOrUpdateIndexerIndex")
            .WithSummary("Creates or updates the Azure Search index for recipes.")
            .WithDescription("Call this only once or when the schema changes.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);


        group.MapPost("/run-indexer", async ([FromServices] IIndexService manager, [FromQuery] string indexerName) =>
        {
            await manager.RunIndexerAsync(indexerName);
            return Results.Ok($"Indexer {indexerName} run triggered.");
        });

        group.MapGet("/status", async ([FromServices] IIndexService manager, [FromQuery] string indexerName) =>
        {
            var status = await manager.GetIndexerStatusAsync(indexerName);
            return Results.Ok(status);
        });

        //TODO: implement others.. 
    }
}

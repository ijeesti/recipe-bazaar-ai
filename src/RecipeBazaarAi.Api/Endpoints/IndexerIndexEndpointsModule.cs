using Carter;
using RecipeBazaarAi.Api.Endpoints.Validators;
using RecipeBazaarAi.Infrastructure.Azure.Interfaces;

namespace RecipeBazaarAi.Api.Endpoints;

public class IndexerIndexEndpointsModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/indexer")
                       .WithTags("Create Index with Indexer");

        group.MapPost("/create", CreateOrUpdateIndex)
             .WithName("CreateOrUpdateIndexerIndex")
             .WithSummary("Creates or updates the Azure Search index for recipes.")
             .WithDescription("Call this only once or when the schema changes.");

        group.MapPost("/run-indexer", RunIndexer)
             .WithName("RunIndexer")
             .WithSummary("Triggers an Azure Search indexer execution")
             .AddEndpointFilter<ValidationFilter<IndexerRequest>>();

        group.MapGet("/status", GetIndexerStatus)
             .WithName("GetIndexerStatus")
             .WithSummary("Fetches execution status and health for a given indexer")
             .AddEndpointFilter<ValidationFilter<IndexerRequest>>();
    }

    private static async Task<IResult> CreateOrUpdateIndex(IRecipeIndexService searchService)
    {
        var success = await searchService.CreateOrUpdateIndexAsync();

        return success
            ? TypedResults.Ok("Index created or updated successfully.")
            : TypedResults.Problem(
                detail: "An error occurred while creating or updating the index.",
                statusCode: StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> RunIndexer(
        [AsParameters] IndexerRequest request,
        IIndexService manager)
    {
        await manager.RunIndexerAsync(request.IndexerName);

        return TypedResults.Ok($"Indexer '{request.IndexerName}' run triggered successfully.");
    }

    private static async Task<IResult> GetIndexerStatus(
        [AsParameters] IndexerRequest request,
        IIndexService manager)
    {
        var status = await manager.GetIndexerStatusAsync(request.IndexerName);

        return status is not null
            ? TypedResults.Ok(status)
            : TypedResults.NotFound($"Indexer '{request.IndexerName}' was not found.");
    }
}
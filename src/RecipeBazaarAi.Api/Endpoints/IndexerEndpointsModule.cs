using RecipeBazaarAi.Infrastructure.Azure.Interfaces;
using Carter;
using global::RecipeBazaarAi.Api.Endpoints.Validators;

namespace RecipeBazaarAi.Api.Endpoints;

public class IndexerEndpointsModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/index")
                       .WithTags("Json Indexer");

        // POST: /api/index/create
        group.MapPost("/create", CreateOrUpdateIndex)
             .WithName("CreateOrUpdateIndex")
             .WithSummary("Creates or updates the Azure Search index for recipes.")
             .WithDescription("Call this only once or when the schema changes.");

        // POST: /api/index/upload-data
        group.MapPost("/upload-data", UploadJsonData)
             .WithName("UploadJsonData")
             .WithSummary("Uploads JSON to Azure Search index.")
             .WithDescription("Upload a JSON file containing recipes.")
             .DisableAntiforgery() // Demo mode
             .AddEndpointFilter<ValidationFilter<FileUploadRequest>>();
    }

    private static async Task<IResult> CreateOrUpdateIndex(IRecipeIndexService searchService)
    {
        var success = await searchService.CreateOrUpdateIndexAsync();

        return success
            ? TypedResults.Ok("Index created or updated successfully.")
            : TypedResults.Problem(
                detail: "Failed to create or update index.",
                statusCode: StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> UploadJsonData(
        [AsParameters] FileUploadRequest request,
        IRecipeIndexService searchService)
    {
        // Validation Filter guarantees request.File is non-null and is a valid JSON file
        using var reader = new StreamReader(request.File.OpenReadStream());
        var json = await reader.ReadToEndAsync();

        var uploaded = await searchService.UploadRecipesAsync(json);

        return uploaded
            ? TypedResults.Ok("Recipes uploaded successfully.")
            : TypedResults.BadRequest("Data already uploaded or contains invalid schema.");
    }
}
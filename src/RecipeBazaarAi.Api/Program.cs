using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using RecipeBaazar.Api.Endpoints;
using RecipeBazaarAi.Infrastructure.Azure;
using RecipeBazaarAi.Infrastructure.Azure.Interfaces;
using RecipeBazaarAi.Infrastructure.Azure.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.AddOpenApi();
//services.AddCors(options =>
//{
//    options.AddPolicy("crs",
//        policy =>
//        {
//            policy.AllowAnyOrigin()
//                  .AllowAnyHeader()
//                  .AllowAnyMethod();
//        });
//});
services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "RecipeBaazar API",
        Version = "v1",
        Description = "API for managing and searching recipes using Azure Search"
    });
});

// Bind once into a concrete object
var azureOptions = builder.Configuration.GetSection("AzureOptions").Get<AzureOptions>();

if (azureOptions is null)
{
    throw new InvalidOperationException("AzureOptions configuration section is missing or invalid.");
}
services.Configure<AzureOptions>(builder.Configuration.GetSection("AzureOptions"));
services.AddSingleton(azureOptions);

// Register SearchClient for index operations
services.AddSingleton(sp =>
{
    return new SearchClient(
        new Uri(azureOptions.Endpoint),
        azureOptions.IndexName,
        new AzureKeyCredential(azureOptions.ApiKey));
});

services.AddSingleton(sp =>
{
    return new SearchIndexClient(
        new Uri(azureOptions.Endpoint),
        new AzureKeyCredential(azureOptions.ApiKey));
});

services.AddSingleton<IRecipeIndexService, RecipeSearchService>();
services.AddSingleton<ICommentIndexService, CommentIndexService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RecipeBaazar API v1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}
//app.UseCors("crs");
// Map endpoints
app.MapRecipeEndpoints();
app.MapIndexEndpoints();
//Index,Indexer Demo => app.MapIndexerIndexEndpoints();

app.Run();

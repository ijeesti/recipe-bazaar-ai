using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Carter;
using RecipeBazaarAi.Infrastructure.Azure;
using RecipeBazaarAi.Infrastructure.Azure.Interfaces;
using RecipeBazaarAi.Infrastructure.Azure.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "Recipe-Bazaar API",
        Version = "v1",
        Description = "API for managing and searching recipes using Azure Search"
    });
});

var azureOptions = builder.Configuration.GetSection(nameof(AzureOptions)).Get<AzureOptions>()
    ?? throw new InvalidOperationException("AzureOptions configuration section is missing or invalid.");

services.Configure<AzureOptions>(builder.Configuration.GetSection(nameof(AzureOptions)));
services.AddSingleton(azureOptions);
services.AddSingleton(sp =>
    new SearchClient(
        new Uri(azureOptions.Endpoint),
        azureOptions.IndexName,
        new AzureKeyCredential(azureOptions.ApiKey)));

services.AddSingleton(sp =>
    new SearchIndexClient(
        new Uri(azureOptions.Endpoint),
        new AzureKeyCredential(azureOptions.ApiKey)));

services.AddSingleton(sp =>
    new SearchIndexerClient(
        new Uri(azureOptions.Endpoint),
        new AzureKeyCredential(azureOptions.ApiKey)));

services.AddSingleton<IRecipeIndexService, RecipeSearchService>();
services.AddSingleton<ICommentIndexService, CommentIndexService>();
services.AddSingleton<IIndexService, IndexService>();
services.AddCarter();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Recipe Bazaar AI API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root "/"
    });
}

app.UseCors();
app.MapCarter();

app.Run();
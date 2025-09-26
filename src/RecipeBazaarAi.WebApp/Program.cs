using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RecipeBazaarAi.WebApp;
using RecipeBazaarAi.WebApp.Interfaces;
using RecipeBazaarAi.WebApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5131/") });
builder.Services.AddScoped<IRecipeWebService, RecipeWebService>();
builder.Services.AddScoped<RecipeState>();

await builder.Build().RunAsync();

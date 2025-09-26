using Microsoft.AspNetCore.Components;
using RecipeBazaarAi.Domain.Contracts;
using RecipeBazaarAi.Domain.Interfaces;
using RecipeBazaarAi.Infrastructure.Azure.Services;


namespace RecipeBazaarAi.WebApp.Components.Recipe;

public class RecipeListBase : ComponentBase
{
    [Inject]
    protected IRecipeAiService RecipeService { get; set; } = default!;

    protected  List<RecipeIndexDto> Recipes { get; set; } = [];
    private int TotalCount { get; set; }
    private int PageSize { get; set; } = 6;
    private int CurrentSkip { get; set; } = 0;
    protected bool IsLoading { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadRecipesAsync();
    }

    private async Task LoadRecipesAsync()
    {
        IsLoading = true;

        // Call service
        var allRecipes = await RecipeService.GetAllRecipesAsync(PageSize);
        Recipes.AddRange(allRecipes);
        CurrentSkip += PageSize;

        IsLoading = false;
    }
}



public class RecipeWebService : IRecipeAiService
{
    private readonly HttpClient _http;

    public RecipeWebService(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<RecipeDto>> GetAllRecipesAsync(int top = 10)
    {
        var response = await _http.GetFromJsonAsync<RecipeResult>($"api/recipes/all?top={top}");
        return response?.Items ?? Enumerable.Empty<RecipeDto>();
    }

    // Implement other interface methods similarly
}


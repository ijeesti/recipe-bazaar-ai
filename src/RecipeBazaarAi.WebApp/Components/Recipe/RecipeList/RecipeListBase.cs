using Microsoft.AspNetCore.Components;
using RecipeBazaarAi.Infrastructure.Azure.Indexes;
using RecipeBazaarAi.WebApp.Interfaces;
using RecipeBazaarAi.WebApp.Services;

namespace RecipeBazaarAi.WebApp.Components.Recipe.RecipeList;

public class RecipeListBase : ComponentBase
{
    [Inject] protected RecipeState RecipeState { get; set; } = default!;
    [Inject] protected IRecipeWebService RecipeService { get; set; } = default!;
    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
    protected RecipeIndexResult RecipeIndexResult { get; set; } = new();
    protected RecipeIndex Recipe { get; set; } = new();
    protected int TotalCount { get; set; }
    protected int PageSize { get; set; } = 6;
    protected int CurrentSkip { get; set; } = 0;
    protected bool IsLoading { get; set; } = true;
    protected bool HasError { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadRecipesAsync();
    }

    private async Task LoadRecipesAsync()
    {
        try
        {
            IsLoading = true;

            // Call service to get paginated recipes
            RecipeIndexResult = await RecipeService.GetAllRecipesAsync(PageSize, CurrentSkip);
            TotalCount = RecipeIndexResult?.TotalCount ?? 0;
            Recipe = RecipeState.SelectedRecipe ?? new(); //TODO: GetRecipeById..
            if (RecipeIndexResult == null || RecipeIndexResult.Recipes.Count == 0)
            {
                HasError = true;
                return;
            }

            HasError = false;
            // Increment skip for next page
            CurrentSkip += PageSize;
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected async Task LoadNext()
    {
        if (CurrentSkip + PageSize >= TotalCount) return;
        CurrentSkip += PageSize;
        await LoadRecipesAsync();
    }

    protected async Task LoadPrevious()
    {
        CurrentSkip = Math.Max(0, CurrentSkip - PageSize);
        await LoadRecipesAsync();
    }

    protected void ViewRecipe(RecipeIndex recipe)
    {
        RecipeState.SelectedRecipe = recipe;
        NavigationManager.NavigateTo($"/recipes/{recipe.Id}", false);
    }
}
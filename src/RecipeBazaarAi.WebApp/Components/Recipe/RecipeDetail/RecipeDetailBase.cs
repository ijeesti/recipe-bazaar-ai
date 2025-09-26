using Microsoft.AspNetCore.Components;
using RecipeBazaarAi.Infrastructure.Azure.Indexes;
using RecipeBazaarAi.WebApp.Interfaces;
using RecipeBazaarAi.WebApp.Services;
namespace RecipeBazaarAi.WebApp.Components.Recipe.RecipeDetail;

public class RecipeDetailBase : ComponentBase
{
    [Parameter] public string Id { get; set; } = string.Empty;
    [Inject] protected IRecipeWebService RecipeService { get; set; } = default!;
    [Inject] protected RecipeState RecipeState { get; set; } = default!;
    protected RecipeIndex? Recipe { get; set; }
    protected bool IsLoading { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        await LoadRecipeAsync();
    }

    protected async Task LoadRecipeAsync()
    {
        try
        {
            IsLoading = true;
            Recipe = RecipeState.SelectedRecipe;//TODO:  await RecipeService.GetByIdAsync(Id);
        }
        finally
        {
            IsLoading = false;
        }
    }

    // called by comment subcomponent
    protected async Task CommentAddedHandler()
    {
        await LoadRecipeAsync();
    }
}


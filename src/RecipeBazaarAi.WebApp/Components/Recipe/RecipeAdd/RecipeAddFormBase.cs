
using Microsoft.AspNetCore.Components;
using RecipeBazaarAi.Domain.Contracts;

namespace RecipeBazaarAi.WebApp.Components.Recipe.RecipeAdd;

public class RecipeAddFormBase : ComponentBase
{
    public RecipeCreateIndexDto NewRecipe { get; set; } = new RecipeCreateIndexDto();

    protected async Task HandleValidSubmit()
    {
        // 1. You would typically call an API or Service to save the data here.
        // Example: await RecipeService.SaveRecipe(NewRecipe);

        Console.WriteLine($"\n--- Recipe Submission Successful ---");
        Console.WriteLine($"Title: {NewRecipe.Title}");
        Console.WriteLine($"Ingredients: {NewRecipe.Ingredients.Length} chars");
        Console.WriteLine($"--- ---------------------------- ---\n");

        // 2. Optionally, reset the form for a new entry or redirect the user.
            // StateHasChanged(); 

        // Example Redirection (if you have NavigationManager injected):
        // NavigationManager.NavigateTo("/recipes"); 
    }
}
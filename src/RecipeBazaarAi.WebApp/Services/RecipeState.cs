using RecipeBazaarAi.Infrastructure.Azure.Indexes;

namespace RecipeBazaarAi.WebApp.Services;

//temporary hack
public class RecipeState
{
    public RecipeIndex? SelectedRecipe { get; set; }
}
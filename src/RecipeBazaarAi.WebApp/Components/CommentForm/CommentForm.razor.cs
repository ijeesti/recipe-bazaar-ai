using Microsoft.AspNetCore.Components;
using RecipeBazaarAi.Infrastructure.Azure.Indexes;
using RecipeBazaarAi.WebApp.Interfaces;
using RecipeBazaarAi.WebApp.Services;

namespace RecipeBazaarAi.WebApp.Components.CommentForm;

public partial class CommentForm : ComponentBase
{
    [Parameter] public string RecipeId { get; set; } = string.Empty;
    [Parameter] public EventCallback OnCommentSubmitted { get; set; }
    [Inject] protected IRecipeWebService RecipeService { get; set; } = default!;
    [Inject] protected RecipeState RecipeState { get; set; } = default!;

    protected CommentIndex NewComment { get; set; } = new()
    {
        Id = Guid.NewGuid().ToString(),
        CommentDate = DateTime.UtcNow
    };

    protected async Task HandleSubmit()
    {
        var ok = await RecipeService.AddCommentAsync(RecipeId, NewComment);
        if (!ok)
        {
            //Do something.
            return;
        }

        NewComment = new CommentIndex
        {
            Id = Guid.NewGuid().ToString(),
            Content = NewComment.Content,
            CommentDate = DateTime.UtcNow
        };
        if (OnCommentSubmitted.HasDelegate)
        {
            await OnCommentSubmitted.InvokeAsync();
        }

        //TODO: DTO mapping.. 
        if (RecipeState.SelectedRecipe is not null)
        {
            var comments = RecipeState.SelectedRecipe.Comments.ToList() ?? [];
            comments.Add(NewComment);
            RecipeState.SelectedRecipe.Comments = [.. comments];
            StateHasChanged();
        }
    }
}

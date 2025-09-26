using Microsoft.AspNetCore.Components;
using RecipeBazaarAi.Infrastructure.Azure.Indexes;

namespace RecipeBazaarAi.WebApp.Components.CommentList;

public partial class CommentList : ComponentBase
{
    [Parameter] public string RecipeId { get; set; } = string.Empty;
    [Parameter] public CommentIndex[]? Comments { get; set; }
    [Parameter] public EventCallback OnCommentAdded { get; set; }

    private readonly string[] AvatarColors = new[]
    {
        "#0d6efd", // Bootstrap Primary Blue
        "#198754", // Bootstrap Success Green
        "#dc3545", // Bootstrap Danger Red
        "#ffc107", // Bootstrap Warning Yellow (dark text needed)
        "#6f42c1", // Bootstrap Purple
        "#20c997", // Bootstrap Teal
        "#fd7e14"  // Bootstrap Orange
    };

    public string GetColorForUser(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            return AvatarColors[0]; // Default color
        }

        // 1. Convert the username to a hash code (an integer).
        // This is a simple, fast way to get a deterministic number from a string.
        int hash = userName.GetHashCode();

        // 2. Ensure the hash code is positive for the modulo operation.
        if (hash < 0)
        {
            hash = -hash;
        }

        // 3. Map the hash code to an index within the color array.
        int index = hash % AvatarColors.Length;

        // 4. Return the color string.
        return AvatarColors[index];
    }

    protected async Task OnFormSubmitted()
    {
        if (OnCommentAdded.HasDelegate)
            await OnCommentAdded.InvokeAsync();
    }
}
namespace RecipeBazaarAi.Domain.Contracts;

public class RecipeDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Ingredients { get; set; } = string.Empty;

    public string Instructions { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public int UserId { get; set; }
    public string? UserName { get; set; } 

    public int CategoryId { get; set; }
    public string? CategoryName { get; set; } 

    public DateTime CreatedOn { get; set; }
}


public class RecipeIndexDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Ingredients { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public CommentDto[] Comments { get; set; } = [];
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}

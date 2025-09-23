
using Azure.Search.Documents.Indexes;

namespace RecipeBazaarAi.Infrastructure.Azure.Indexes;

public class RecipeIndex
{
    [SimpleField(IsKey = true, IsFilterable = true)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [SearchableField(IsFilterable = true, IsSortable = true)]
    public string Title { get; set; } = string.Empty;

    [SearchableField(IsFilterable = true, IsSortable = true)]
    public string Description { get; set; } = string.Empty;

    [SearchableField(IsFilterable = false, IsSortable = false)]
    public string Ingredients { get; set; } = string.Empty;

    [SearchableField(IsFilterable = false, IsSortable = false)]
    public string Instructions { get; set; } = string.Empty;

    [SimpleField(IsFilterable = false, IsSortable = false)]
    public string UserName { get; set; } = string.Empty;

    [SimpleField(IsFilterable = true, IsSortable = true)]
    public string CategoryName { get; set; } = string.Empty;

    [SearchableField(IsFilterable = false)]
    public CommentIndex[] Comments { get; set; } = [];

    [SimpleField(IsFilterable = true, IsSortable = false)]
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}
using Azure.Search.Documents.Indexes;

namespace RecipeBazaarAi.Infrastructure.Azure.Indexes;

public class CommentIndex
{
    [SimpleField(IsKey = false, IsFilterable = true)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [SearchableField]
    public string Content { get; set; } = string.Empty;

    [SearchableField(IsFilterable = false, IsSortable = false)]
    public string UserName { get; set; } = string.Empty;

    [SimpleField(IsFilterable = false, IsSortable = false)]
    public DateTime CommentDate { get; set; } = DateTime.UtcNow;
}
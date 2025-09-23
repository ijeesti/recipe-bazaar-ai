namespace RecipeBazaarAi.Domain.Contracts;

public class SearchRequest
{
    public int Top { get; set; }
    public string Query { get; set; } = string.Empty;
}

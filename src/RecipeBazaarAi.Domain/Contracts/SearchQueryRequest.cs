namespace RecipeBazaarAi.Domain.Contracts;

public record SearchQueryRequest
{
    public string Query { get; init; } = string.Empty;

    public int Top
    {
        get => _top;
        init => _top = value > 0 ? Math.Min(value, 100) : 10;
    }

    private readonly int _top = 10;
}


public record PaginationRequest(int Skip = 0, int Take = 10);
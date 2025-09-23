using RecipeBazaarAi.Infrastructure.Azure.Indexes;

namespace RecipeBazaarAi.Infrastructure.Azure.Interfaces;

public interface ICommentIndexService
{
    Task<bool> AddCommentAsync(string recipeId, CommentIndex newComment);
}


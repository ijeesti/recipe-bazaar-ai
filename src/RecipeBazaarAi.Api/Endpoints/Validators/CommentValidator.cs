using FluentValidation;
using RecipeBazaarAi.Infrastructure.Azure.Indexes;

namespace RecipeBazaarAi.Api.Endpoints.Validators;

public class CommentValidator : AbstractValidator<CommentIndex>
{
    public CommentValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("User name is required.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Comment body cannot be empty.")
            .MaximumLength(1000).WithMessage("Comment exceeds maximum length.");
    }
}
using FluentValidation;
using RecipeBazaarAi.Domain.Contracts;

namespace RecipeBazaarAi.Api.Endpoints.Validators;

public class SearchQueryValidator : AbstractValidator<SearchQueryRequest>
{
    public SearchQueryValidator()
    {
        RuleFor(x => x.Query)
            .NotEmpty().WithMessage("Query cannot be empty.")
            .MinimumLength(3).WithMessage("Query must be at least 3 characters.");

        RuleFor(x => x.Top)
            .GreaterThanOrEqualTo(1).WithMessage("Top must be at least 1.")
            .LessThanOrEqualTo(100).WithMessage("Top cannot exceed 100.");
    }
}

using FluentValidation;

namespace RecipeBazaarAi.Api.Endpoints.Validators;

public record IndexerRequest(string IndexerName);

public class IndexerRequestValidator : AbstractValidator<IndexerRequest>
{
    public IndexerRequestValidator()
    {
        RuleFor(x => x.IndexerName)
            .NotEmpty().WithMessage("Indexer name is required.")
            .MaximumLength(128).WithMessage("Indexer name cannot exceed 128 characters.");
    }
}


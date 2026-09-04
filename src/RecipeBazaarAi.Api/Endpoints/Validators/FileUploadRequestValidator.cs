using FluentValidation;

namespace RecipeBazaarAi.Api.Endpoints.Validators;

public record FileUploadRequest(IFormFile File);

public class FileUploadRequestValidator : AbstractValidator<FileUploadRequest>
{
    public FileUploadRequestValidator()
    {
        RuleFor(x => x.File)
            .NotNull().WithMessage("No file uploaded.")
            .Must(file => file is not null && file.Length > 0).WithMessage("Uploaded file cannot be empty.")
            .Must(file => file is not null && file.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Only JSON files are supported.");
    }
}
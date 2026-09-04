using FluentValidation;

namespace RecipeBazaarAi.Api.Endpoints.Validators;

public class ValidationFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

        if (validator is not null)
        {
            var argumentToValidate = context.Arguments.OfType<T>().FirstOrDefault();

            if (argumentToValidate is null)
            {
                return TypedResults.BadRequest("Request payload or parameters cannot be null.");
            }

            var validationResult = await validator.ValidateAsync(argumentToValidate);

            if (!validationResult.IsValid)
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }
        }

        return await next(context);
    }
}
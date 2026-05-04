using FluentValidation.Results;

namespace FCG.API.Extensions;

public static class ValidationResultExtensions
{
    public static IResult ToBadRequestValidationProblem(this ValidationResult validation)
    {
        return Results.ValidationProblem(validation.ToErrors(), statusCode: StatusCodes.Status400BadRequest);
    }

    public static Dictionary<string, string[]> ToErrors(this ValidationResult validation)
    {
        return validation.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray()
            );
    }
}

using FluentValidation.Results;

namespace BenchmarkWebApi.Extensions;

public static class ValidationResultExtensions
{
  extension(ValidationResult validationResult)
  {
    /// <summary>
    /// Converts the current validation result to a Minimal API-compatible result object representing validation errors.
    /// </summary>
    /// <returns>An <see cref="IResult"/> that encapsulates the validation errors in a format suitable for Minimal APIs.</returns>
    public IResult ToMinimalApiResult()
    {
      var errors = GetErrors(validationResult);
      return Results.ValidationProblem(errors);
    }
  }

  private static Dictionary<string, string[]> GetErrors(ValidationResult validationResult)
  {
    return validationResult.Errors
      .GroupBy(e => e.PropertyName)
      .Select(g => new { g.Key, Values = g.Select(e => e.ErrorMessage).ToArray() })
      .ToDictionary(pair => pair.Key, pair => pair.Values);
  }
}

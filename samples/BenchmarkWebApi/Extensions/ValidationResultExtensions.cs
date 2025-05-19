using FluentValidation.Results;

namespace BenchmarkWebApi.Extensions;

public static class ValidationResultExtensions
{
  public static IResult ToMinimalApiResult(this ValidationResult validationResult)
  {
    var errors = GetErrors(validationResult);
    return Results.ValidationProblem(errors);
  }

  private static Dictionary<string, string[]> GetErrors(ValidationResult validationResult)
  {
    return validationResult.Errors
      .GroupBy(e => e.PropertyName)
      .Select(g => new { g.Key, Values = g.Select(e => e.ErrorMessage).ToArray() })
      .ToDictionary(pair => pair.Key, pair => pair.Values);
  }
}

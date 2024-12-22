using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace ModEndpoints.Core;
public static class ValidationResultExtensions
{
  public static IResult ToMinimalApiResult(this ValidationResult validationResult)
  {
    var errors = validationResult.Errors
      .GroupBy(e => e.PropertyName)
      .Select(g => new { g.Key, Values = g.Select(e => e.ErrorMessage).ToArray() })
      .ToDictionary(pair => pair.Key, pair => pair.Values);
    return Results.ValidationProblem(errors);
  }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ModEndpoints.Core;
public static class ValidationResultExtensions
{
  public static IResult ToMinimalApiResult(this RequestValidationResult validationResult)
  {
    var errors = GetErrors(validationResult);
    return Results.ValidationProblem(errors);
  }
  public static ValidationProblem ToTypedValidationProblem(this RequestValidationResult validationResult)
  {
    var errors = GetErrors(validationResult);
    return TypedResults.ValidationProblem(errors);
  }
  public static ProblemHttpResult ToTypedProblem(this RequestValidationResult validationResult)
  {
    var errors = GetErrors(validationResult);
    return TypedResults.Problem(new HttpValidationProblemDetails(errors));
  }
  public static BadRequest<HttpValidationProblemDetails> ToTypedBadRequestWithValidationProblem(this RequestValidationResult validationResult)
  {
    var errors = GetErrors(validationResult);
    return TypedResults.BadRequest(new HttpValidationProblemDetails(errors));
  }
  public static BadRequest<ProblemDetails> ToTypedBadRequestWithProblem(this RequestValidationResult validationResult)
  {
    var errors = GetErrors(validationResult);
    return TypedResults.BadRequest((ProblemDetails)new HttpValidationProblemDetails(errors));
  }

  private static Dictionary<string, string[]> GetErrors(RequestValidationResult validationResult)
  {
    return validationResult.Errors
      .GroupBy(e => e.PropertyName)
      .Select(g => new { g.Key, Values = g.Select(e => e.ErrorMessage).ToArray() })
      .ToDictionary(pair => pair.Key, pair => pair.Values);
  }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ModEndpoints.Core;

public static class RequestValidationResultExtensions
{
  extension(RequestValidationResult validationResult)
  {
    public IResult ToMinimalApiResult()
    {
      var errors = GetErrors(validationResult);
      return Results.ValidationProblem(errors);
    }
    public ValidationProblem ToTypedValidationProblem()
    {
      var errors = GetErrors(validationResult);
      return TypedResults.ValidationProblem(errors);
    }
    public ProblemHttpResult ToTypedProblem()
    {
      var errors = GetErrors(validationResult);
      return TypedResults.Problem(new HttpValidationProblemDetails(errors));
    }
    public BadRequest<HttpValidationProblemDetails> ToTypedBadRequestWithValidationProblem()
    {
      var errors = GetErrors(validationResult);
      return TypedResults.BadRequest(new HttpValidationProblemDetails(errors));
    }
    public BadRequest<ProblemDetails> ToTypedBadRequestWithProblem()
    {
      var errors = GetErrors(validationResult);
      return TypedResults.BadRequest((ProblemDetails)new HttpValidationProblemDetails(errors));
    }
  }

  private static Dictionary<string, string[]> GetErrors(RequestValidationResult validationResult)
  {
    return validationResult.Errors?
      .GroupBy(e => e.PropertyName)
      .Select(g => new { g.Key, Values = g.Select(e => e.ErrorMessage).ToArray() })
      .ToDictionary(pair => pair.Key, pair => pair.Values) ?? [];
  }
}

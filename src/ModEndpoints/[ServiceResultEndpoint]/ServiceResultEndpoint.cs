using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using ModEndpoints.Core;
using ModResults;
using ModResults.FluentValidation;

namespace ModEndpoints;
public abstract class ServiceResultEndpoint<TRequest, TResultValue>
  : BaseServiceResultEndpoint<TRequest, Result<TResultValue>>
  where TRequest : notnull
  where TResultValue : notnull
{
  protected override ValueTask<Result<TResultValue>> HandleInvalidValidationResultAsync(
    ValidationResult validationResult,
    HttpContext context,
    CancellationToken ct)
  {
    return new ValueTask<Result<TResultValue>>(
      validationResult.ToInvalidResult<TResultValue>());
  }
}

public abstract class ServiceResultEndpoint<TRequest>
  : BaseServiceResultEndpoint<TRequest, Result>
  where TRequest : notnull
{
  protected override ValueTask<Result> HandleInvalidValidationResultAsync(
    ValidationResult validationResult,
    HttpContext context,
    CancellationToken ct)
  {
    return new ValueTask<Result>(
      validationResult.ToInvalidResult());
  }
}

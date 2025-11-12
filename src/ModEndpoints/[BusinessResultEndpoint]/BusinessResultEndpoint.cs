using Microsoft.AspNetCore.Http;
using ModEndpoints.Core;
using ModResults;

namespace ModEndpoints;

/// <summary>
/// Abstract base class for endpoints that return a <see cref="Result{TResultValue}"/> business result from HandleAsync method wrapped in an HTTP 200 <see cref="IResult"/>.
/// </summary>
/// <typeparam name="TRequest">Request type.</typeparam>
/// <typeparam name="TResultValue">Type of the value contained by business result response.</typeparam>
public abstract class BusinessResultEndpoint<TRequest, TResultValue>
  : BaseBusinessResultEndpoint<TRequest, Result<TResultValue>>
  where TRequest : notnull
  where TResultValue : notnull
{
  protected override ValueTask<Result<TResultValue>> HandleInvalidValidationResultAsync(
    RequestValidationResult validationResult,
    HttpContext context,
    CancellationToken ct)
  {
    return new ValueTask<Result<TResultValue>>(
      validationResult.ToInvalidResult<TResultValue>());
  }
}

/// <summary>
/// Abstract base class for endpoints that return a <see cref="Result"/> business result from HandleAsync method wrapped in an HTTP 200 <see cref="IResult"/>.
/// </summary>
/// <typeparam name="TRequest">Request type.</typeparam>
public abstract class BusinessResultEndpoint<TRequest>
  : BaseBusinessResultEndpoint<TRequest, Result>
  where TRequest : notnull
{
  protected override ValueTask<Result> HandleInvalidValidationResultAsync(
    RequestValidationResult validationResult,
    HttpContext context,
    CancellationToken ct)
  {
    return new ValueTask<Result>(
      validationResult.ToInvalidResult());
  }
}

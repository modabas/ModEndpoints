using Microsoft.AspNetCore.Http;
using ModEndpoints.Core;
using ModResults;

namespace ModEndpoints;

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

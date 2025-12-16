using Microsoft.AspNetCore.Http;
using ModEndpoints.Core;
using ModResults;

namespace ModEndpoints;

/// <summary>
/// Abstract base class for endpoints that convert a <see cref="Result{TResponse}"/> business result returned from HandleAsync method to an <see cref="IResult"/> HTTP response, depending on the business result type, state and failure type (if any).
/// </summary>
/// <typeparam name="TRequest">Request type.</typeparam>
/// <typeparam name="TResponse">Type of the value contained by business result response.</typeparam>
public abstract class WebResultEndpoint<TRequest, TResponse>
  : BaseWebResultEndpoint<TRequest, WebResult<TResponse>>
  where TRequest : notnull
  where TResponse : notnull
{
  protected override ValueTask<IResult> ConvertResultToResponseAsync(
    WebResult<TResponse> result,
    HttpContext context,
    CancellationToken ct)
  {
    return result.ExecuteAsync(context, ct);
  }

  protected override ValueTask<IResult> HandleInvalidValidationResultAsync(
    RequestValidationResult validationResult,
    HttpContext context,
    CancellationToken ct)
  {
    var invalidResult = validationResult.ToInvalidResult<TResponse>();
    return ConvertResultToResponseAsync(invalidResult, context, ct);
  }
}

/// <summary>
/// Abstract base class for endpoints that convert a <see cref="Result"/> business result returned from HandleAsync method to an <see cref="IResult"/> HTTP response, depending on the business result type, state and failure type (if any).
/// </summary>
/// <typeparam name="TRequest">Request type.</typeparam>
public abstract class WebResultEndpoint<TRequest>
  : BaseWebResultEndpoint<TRequest, WebResult>
  where TRequest : notnull
{
  protected override ValueTask<IResult> ConvertResultToResponseAsync(
    WebResult result,
    HttpContext context,
    CancellationToken ct)
  {
    return result.ExecuteAsync(context, ct);
  }

  protected override ValueTask<IResult> HandleInvalidValidationResultAsync(
    RequestValidationResult validationResult,
    HttpContext context,
    CancellationToken ct)
  {
    var invalidResult = validationResult.ToInvalidResult();
    return ConvertResultToResponseAsync(invalidResult, context, ct);
  }
}

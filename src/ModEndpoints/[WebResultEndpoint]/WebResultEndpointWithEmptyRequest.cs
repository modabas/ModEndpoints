using Microsoft.AspNetCore.Http;
using ModEndpoints.Core;
using ModResults;

namespace ModEndpoints;

/// <summary>
/// Abstract base class for endpoints that convert a <see cref="Result{TResponse}"/> business result returned from HandleAsync method to an <see cref="IResult"/> HTTP response, depending on the business result type, state and failure type (if any).
/// </summary>
/// <typeparam name="TResponse">Type of the value contained by business result response.</typeparam>
public abstract class WebResultEndpointWithEmptyRequest<TResponse>
  : BaseWebResultEndpoint<WebResult<TResponse>>
  where TResponse : notnull
{
  protected override ValueTask<IResult> ConvertResultToResponseAsync(
    WebResult<TResponse> result,
    HttpContext context,
    CancellationToken ct)
  {
    return result.ExecuteAsync(context, ct);
  }
}

/// <summary>
/// Abstract base class for endpoints that convert a <see cref="Result"/> business result returned from HandleAsync method to an <see cref="IResult"/> HTTP response, depending on the business result type, state and failure type (if any).
/// </summary>
public abstract class WebResultEndpointWithEmptyRequest
  : BaseWebResultEndpoint<WebResult>
{
  protected override ValueTask<IResult> ConvertResultToResponseAsync(
    WebResult result,
    HttpContext context,
    CancellationToken ct)
  {
    return result.ExecuteAsync(context, ct);
  }
}
